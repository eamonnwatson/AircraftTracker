using AircraftTracker.Entities;
using MediatR;

namespace AircraftTracker.Commands.Alerts;
internal record UpdateAlertsCommand(IEnumerable<Alert> Alerts) : IRequest;
