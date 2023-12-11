using AircraftTracker.Commands.Alerts;
using AircraftTracker.Commands.Identified;
using AircraftTracker.Commands.LiveFlights;
using AircraftTracker.Entities;
using AircraftTracker.Queries.Alerts;
using AircraftTracker.Queries.FlightAware;
using AircraftTracker.Queries.Identified;
using AircraftTracker.Queries.LiveFlights;
using LiteDB;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Quartz;

namespace AircraftTracker.Job;
[DisallowConcurrentExecution]
internal class FlightChecker(ILogger<FlightChecker> logger, 
                             ISender sender) : IJob
{
    private readonly ILogger<FlightChecker> _logger = logger;
    private readonly ISender _sender = sender;
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var result = await _sender.Send(new RequestLiveFlightsFromWeb(), context.CancellationToken);
            var flightsFromWeb = result.Value;
            var flightsFromDB = await _sender.Send(new RequestLiveFlightsFromDatabase(), context.CancellationToken);
            var alerts = await _sender.Send(new RequestAlertsFromDatabase(), context.CancellationToken);
            var identified = await _sender.Send(new RequestIdentifiedFromDatabase(), context.CancellationToken);

            _logger.LogInformation("{NumberFlights} Flights found on web", flightsFromWeb.Count());

            if (flightsFromDB.IsFailed || alerts.IsFailed || identified.IsFailed) return;

            var newFlights = GetNewFlights(flightsFromDB.Value, flightsFromWeb);
            var oldFlights = GetOldFlights(flightsFromDB.Value, flightsFromWeb);

            var newTypes = newFlights.ExceptBy(identified.Value.Select(i => i.AircraftType), f => f.Type).DistinctBy(a => a.Type).Select(f => Identified.Create(f.Type));
            var updatedTypes = identified.Value.IntersectBy(newFlights.Select(f => f.Type), i => i.AircraftType).Select(UpdateIdentified);
            var updatedAlerts = alerts.Value.IntersectBy(newFlights.Select(f => f.Type), a => a.AircraftType).Select(UpdateAlerts);

            _logger.LogInformation("{NumberFlights} New Flights Found", newFlights.Count);
            _logger.LogInformation("{NumberTypes} New Types Found", newTypes.Count());
            _logger.LogInformation("{NumberAlerts} Alert Types Found", updatedAlerts.Count());

            // Update LiveFlightsTable
            await _sender.Send(new DeleteLiveFlightsCommand(oldFlights));
            await _sender.Send(new AddLiveFlightsCommand(newFlights));

            //Update Identified Table
            await _sender.Send(new UpdatedIdentifiedCommand(updatedTypes));
            await _sender.Send(new AddIdentifiedCommand(newTypes));

            //Update Alerts Table
            await _sender.Send(new UpdateAlertsCommand(updatedAlerts));

            //Send Notifications
            await _sender.Send(new SendAlertsNotificationCommand(newFlights.IntersectBy(alerts.Value.Select(a => a.AircraftType), f => f.Type)));
            await _sender.Send(new SendNewFlightAlertCommand(newFlights.ExceptBy(identified.Value.Select(i => i.AircraftType), f => f.Type)));

        }
        catch (BrokenCircuitException ex)
        {
            await context.Scheduler.Shutdown();
            _logger.LogCritical(ex, "Application Shutting down, too many failures");
            Environment.Exit(-1);
        }
        catch (LiteException ex)
        {
            await context.Scheduler.Shutdown();
            _logger.LogCritical(ex, "Database Error");
            Environment.Exit(-1);
        }
    }

    private static Alert UpdateAlerts(Alert alert)
    {
        alert.UpdateAlert();
        return alert;
    }

    private static Identified UpdateIdentified(Identified identified)
    {
        identified.LastViewed = DateTime.UtcNow;
        return identified;
    }

    private static List<LiveFlight> GetNewFlights(IEnumerable<LiveFlight> flightsFromDB, IEnumerable<LiveFlight> flightsFromWeb)
        => flightsFromWeb.ExceptBy(flightsFromDB.Select(f => f.Link), f => f.Link).ToList();

    private static List<LiveFlight> GetOldFlights(IEnumerable<LiveFlight> flightsFromDB, IEnumerable<LiveFlight> flightsFromWeb)
        => flightsFromDB.ExceptBy(flightsFromWeb.Select(f => f.Link), f => f.Link).ToList();

}
