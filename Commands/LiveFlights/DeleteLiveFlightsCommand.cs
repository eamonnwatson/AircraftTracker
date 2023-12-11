using AircraftTracker.Entities;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal record DeleteLiveFlightsCommand(IEnumerable<LiveFlight> Flights) : IRequest;
