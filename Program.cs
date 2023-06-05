using AircraftTracker;
using AircraftTracker.Interfaces;
using AircraftTracker.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureAppConfiguration(config =>
{
    config.AddJsonFile("Options/AircraftOptions.json", false, true);
});

builder.ConfigureLogging((hostContext, cfg) =>
{
    cfg.AddSimpleConsole(opt =>
    {
        opt.SingleLine = true;
        opt.UseUtcTimestamp = true;
        opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
        opt.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        opt.IncludeScopes = false;
    });
});

builder.ConfigureServices((hostContext, services) =>
{
    var emailHost = hostContext.Configuration.GetValue("EMAIL_SERVER", string.Empty);
    var emailPort = hostContext.Configuration.GetValue("EMAIL_PORT", 587);
    var emailUsername = hostContext.Configuration.GetValue("EMAIL_USERNAME", string.Empty);
    var emailPassword = hostContext.Configuration.GetValue("EMAIL_PASSWORD", string.Empty);
    var emailTo = hostContext.Configuration.GetValue<string>("EMAIL_FROM") ?? throw new Exception("EMAIL_FROM Parameter not provided");

    services.AddFluentEmail(emailTo).AddSmtpSender(emailHost, emailPort, emailUsername, emailPassword);
    services.AddTransient<IFlightParser, FlightAware>();
    services.AddTransient<INotificationService, EmailNotification>();
    services.AddSingleton<IFlightStorage, FlightStorage>();
    services.AddHostedService<FlightChecker>();
    services.ConfigureWritable<AircraftOptions>(hostContext.Configuration.GetSection("AircraftOptions"), "Options/AircraftOptions.json");
});

var app = builder.Build();

await app.StartAsync();
