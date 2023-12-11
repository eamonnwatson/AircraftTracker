using AircraftTracker.Entities;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.Alerts;
internal record RequestAlertsFromDatabase() : IRequest<Result<IEnumerable<Alert>>>;
