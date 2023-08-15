using AircraftTracker;
using AircraftTracker.Interfaces;
using AircraftTracker.Options;
using AircraftTracker.Pushover;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    var PO_AppKey = hostContext.Configuration.GetValue<string>("PO_AppKey") ?? throw new Exception("PO_AppKey Parameter not provided");
    var PO_UserKey = hostContext.Configuration.GetValue<string>("PO_UserKey");

    services.AddTransient<IPushoverClient, PushoverClient>(s => new PushoverClient(PO_AppKey, PO_UserKey));
    services.AddTransient<IFlightParser, FlightAware>();
    services.AddTransient<INotificationService, PushoverNotification>();
    services.AddSingleton<IFlightStorage, FlightStorage>();
    services.AddHostedService<FlightChecker>();
    services.ConfigureWritable<AircraftOptions>(hostContext.Configuration.GetSection("AircraftOptions"), "Options/AircraftOptions.json");
});

var app = builder.Build();

await app.StartAsync();
