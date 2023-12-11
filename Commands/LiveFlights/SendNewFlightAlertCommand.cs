using AircraftTracker.Entities;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal record SendNewFlightAlertCommand(IEnumerable<LiveFlight> Flights) : IRequest;

