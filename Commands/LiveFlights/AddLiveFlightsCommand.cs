using AircraftTracker.Entities;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal record AddLiveFlightsCommand(IEnumerable<LiveFlight> Flights) : IRequest;
