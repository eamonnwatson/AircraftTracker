using AircraftTracker.Entities;
using MediatR;

namespace AircraftTracker.Commands.Alerts;
internal record SendAlertsNotificationCommand(IEnumerable<LiveFlight> Flights) : IRequest;

