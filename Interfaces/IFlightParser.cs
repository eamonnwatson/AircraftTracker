using AircraftTracker.Entities;
using FluentResults;

namespace AircraftTracker.Interfaces;
internal interface IFlightParser
{
    Task<Result<IEnumerable<LiveFlight>>> GetFlightsAsync(string airportICAO, CancellationToken cancellationToken = default);
}