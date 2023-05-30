using AircraftTracker.Interfaces;
using AircraftTracker.Models;
using Microsoft.Extensions.Logging;

namespace AircraftTracker;
internal class FlightStorage : IFlightStorage
{
    private readonly List<LiveFlight> flights = new();
    private readonly ILogger<FlightStorage> logger;

    public FlightStorage(ILogger<FlightStorage> logger)
    {
        this.logger = logger;
    }

    public IEnumerable<LiveFlight> AddFlights(IEnumerable<LiveFlight> newflights)
    {
        var removeflights = new List<LiveFlight>();
        foreach (var flight in flights)
        {
            if (!newflights.Any(a => a.link == flight.link))
                removeflights.Add(flight);
        }

        foreach (var flight in removeflights)
        {
            flights.Remove(flight);
        }

        logger.LogInformation("Removed {RemoveCount} Flights", removeflights.Count);

        var outputList = new List<LiveFlight>();

        foreach (var flight in newflights)
        {
            var foundFlight = flights.FirstOrDefault(f => f.link == flight.link);

            if (foundFlight is not null)
                flights.Remove(foundFlight);
            else
                outputList.Add(flight);

            flights.Add(flight);
        }

        logger.LogInformation("{NewCount} New Flights", outputList.Count);

        return outputList;
    }
}
