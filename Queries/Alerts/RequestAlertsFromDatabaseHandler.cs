using AircraftTracker.Entities;
using AircraftTracker.Interfaces;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.Alerts;
internal class RequestAlertsFromDatabaseHandler(IRepository repository) : IRequestHandler<RequestAlertsFromDatabase, Result<IEnumerable<Alert>>>
{
    private readonly IRepository _repository = repository;

    public Task<Result<IEnumerable<Alert>>> Handle(RequestAlertsFromDatabase request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository.GetFlightAlerts());
    }
}
