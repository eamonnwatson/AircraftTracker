using AircraftTracker.Interfaces;
using AircraftTracker.Models;
using AircraftTracker.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AircraftTracker;
internal class FlightChecker : IHostedService
{
    private readonly ILogger<FlightChecker> logger;
    private readonly IFlightParser liveFlights;
    private readonly IFlightStorage storage;
    private readonly IWritableOptions<AircraftOptions> options;
    private readonly INotificationService notification;
    private readonly int frequency;
    private readonly string airport;

    public FlightChecker(IConfiguration config,
                         ILogger<FlightChecker> logger,
                         IFlightParser liveFlights,
                         IFlightStorage storage,
                         IWritableOptions<AircraftOptions> options,
                         INotificationService notification)
    {
        this.logger = logger;
        this.liveFlights = liveFlights;
        this.storage = storage;
        this.options = options;
        this.notification = notification;

        frequency = config.GetValue("FREQUENCY", 3600);
        airport = config.GetValue("AIRPORT", string.Empty)!;

        logger.LogInformation("Frequency of updates : {frequency} sec", frequency);

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var numErrors = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogDebug("Alert Types : {AlertTypes}", string.Join(',', options.Value.AlertTypes));
                logger.LogDebug("Identified Types : {IdentifiedTypes}", string.Join(',', options.Value.IdentifiedTypes));

                var flights = await liveFlights.GetFlightsAsync(airport, cancellationToken); // Gets Flights from FlightAware
                var newFlights = storage.AddFlights(flights); // Adds them to persistant Storage and returns any that are new
                var newTypesFlights = CheckForNewTypes(newFlights); // Checks new flights against types already known.
                var alertFlights = CheckForAlerts(newFlights); // Checks new flights against types to be alerted.

                if (newTypesFlights.Any() || alertFlights.Any())
                {
                    StringBuilder body = new();

                    if (newTypesFlights.Any())
                    {
                        logger.LogInformation("{newTypes} New Flight Types Found : {types}", newTypesFlights.Count(), string.Join(',',newTypesFlights.Select(a => a.Type)));

                        body.AppendLine("The following new flights have been found with new aircraft types : ");
                        body.AppendLine();
                        foreach (var flight in newTypesFlights)
                        {
                            body.AppendLine($"   {flight.Ident} ({flight.Type} / {flight.FullType}) - {flight.From} - {flight.Arrive}");
                        }
                        body.AppendLine();
                        body.AppendLine();
                    }

                    if (alertFlights.Any())
                    {
                        logger.LogInformation("{newAlerts} Alert Flight Types Found : {types}", alertFlights.Count(), string.Join(',', alertFlights.Select(a => a.Type)));

                        body.AppendLine("The following new flights have been found with alerted flight types : ");
                        body.AppendLine();
                        foreach (var flight in alertFlights)
                        {
                            body.AppendLine($"   {flight.Ident} ({flight.Type} / {flight.FullType}) - {flight.From} - {flight.Arrive}");
                        }
                    }

                    await notification.SendNotificationAsync(body.ToString(), cancellationToken);
                    logger.LogInformation("Email Sent");
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Flight Checking stopped");
                return;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error occured checking flights");
                numErrors++;

                if (numErrors > 3)
                {
                    logger.LogCritical("3 Errors received, terminating application");
                    return;
                }
            }

            await Task.Delay(1000 * frequency, cancellationToken);
        }
    }

    private IEnumerable<LiveFlight> CheckForAlerts(IEnumerable<LiveFlight> newFlights)
    {
        return newFlights.Where(a => options.Value.AlertTypes.Contains(a.Type));
    }

    private IEnumerable<LiveFlight> CheckForNewTypes(IEnumerable<LiveFlight> newFlights)
    {
        var newTypes = newFlights.Select(a => a.Type);
        var items = newTypes.Except(options.Value.IdentifiedTypes);

        if (items.Any())
            options.Update(opt => 
            {
                opt.IdentifiedTypes.AddRange(items);
                opt.IdentifiedTypes = opt.IdentifiedTypes.Distinct().OrderBy(a => a).ToList();

                logger.LogDebug("Added Types : {NewTypes}", string.Join(',', items));
            });

        return newFlights.Where(a => items.Contains(a.Type));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Flight Checking stopped");
        return Task.CompletedTask;
    }
}