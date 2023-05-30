using AircraftTracker.Models;

namespace AircraftTracker.Interfaces;
internal interface IFlightStorage
{
    IEnumerable<LiveFlight> AddFlights(IEnumerable<LiveFlight> newflights);
}