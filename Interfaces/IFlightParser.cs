using AircraftTracker.Models;

namespace AircraftTracker.Interfaces;
internal interface IFlightParser
{
    Task<IEnumerable<LiveFlight>> GetFlightsAsync(string airportICAO, CancellationToken cancellationToken);
}