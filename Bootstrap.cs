using AircraftTracker.Entities;
using AircraftTracker.Interfaces;
using AircraftTracker.Job;
using AircraftTracker.LiteDB;
using AircraftTracker.Options;
using AircraftTracker.Parser;
using AircraftTracker.Pushover;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Quartz;
using Serilog;
using Serilog.Events;

namespace AircraftTracker;
internal static class Bootstrap
{
    public static void ConfigureStatupServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        var airport = hostContext.Configuration.GetValue<string>("airport") ??
            throw new ArgumentException("Airport Environment Variable NOT SET");

#if DEBUG
        services.AddSerilog(config => config.ReadFrom.Configuration(hostContext.Configuration));
#else
        if (string.IsNullOrWhiteSpace(hostContext.Configuration.GetValue<string>("Pushover_APIToken")))
            throw new ArgumentException("Pushover_APIToken Environment Variable NOT SET");

        if (string.IsNullOrWhiteSpace(hostContext.Configuration.GetValue<string>("Pushover_UserKey")))
            throw new ArgumentException("Pushover_UserKey Environment Variable NOT SET");

        var seqURL = hostContext.Configuration.GetValue<string>("SEQ_URL") ??
            throw new ArgumentException("Seq URL Variable NOT SET");

        var seqKey = hostContext.Configuration.GetValue<string>("SEQ_KEY") ??
            throw new ArgumentException("Seq API Key Variable NOT SET");

        services.AddSerilog(config =>
        {
            config.MinimumLevel.Warning()
                  .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                  .MinimumLevel.Override("AircraftTracker", LogEventLevel.Information)
                  .Enrich.FromLogContext()
                  .WriteTo.Seq(seqURL, apiKey: seqKey)
                  .WriteTo.Console();
        });
#endif

        services.AddOptions<CheckerOptions>().BindConfiguration(nameof(CheckerOptions));
        var provider = services.BuildServiceProvider();

        var opt = provider.GetRequiredService<IOptions<CheckerOptions>>();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Bootstrap");
        logger.LogInformation("Airport: {Airport}", airport);
        logger.LogInformation("Frequency of Updates: {Frequency}", opt.Value.FrequencyOfCheckSeconds);


        services.AddScoped<IFlightParser, FlightAware>();
        services.AddScoped<IRepository, Repository>();
        services.AddLiteDatabase();
        services.AddPushover();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddResiliencePipeline<string, Result<IEnumerable<LiveFlight>>>("flight-pipeline", (builder, context) =>
        {
            var predicate = new PredicateBuilder<Result<IEnumerable<LiveFlight>>>()
                    .HandleResult(result => result.IsFailed == true);

            builder.InstanceName = "mainpipeline";

            builder.AddCircuitBreaker(new()
            {
                SamplingDuration = TimeSpan.FromHours(12),
                ShouldHandle = predicate,
                BreakDuration = TimeSpan.FromMinutes(5),
                MinimumThroughput = 10
            });

            builder.AddRetry(new()
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(5),
                UseJitter = true,
                ShouldHandle = predicate
            });
        });

        services.AddQuartz(cfg =>
        {
            var jobKey = JobKey.Create(nameof(FlightChecker));
            cfg.AddJob<FlightChecker>(jobKey);
            cfg.AddTrigger(cfg =>
                                cfg.ForJob(jobKey)
                                   .StartNow()
                                   .WithSimpleSchedule(builder =>
                                      builder.WithIntervalInSeconds(opt.Value.FrequencyOfCheckSeconds).RepeatForever()));
        });

        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    }

}
