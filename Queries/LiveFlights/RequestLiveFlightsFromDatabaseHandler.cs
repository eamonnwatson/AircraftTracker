using AircraftTracker.Entities;
using AircraftTracker.Interfaces;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.LiveFlights;
internal class RequestLiveFlightsFromDatabaseHandler(IRepository repository) : IRequestHandler<RequestLiveFlightsFromDatabase, Result<IEnumerable<LiveFlight>>>
{
    private readonly IRepository _repository = repository;

    public Task<Result<IEnumerable<LiveFlight>>> Handle(RequestLiveFlightsFromDatabase request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository.GetLiveFlights());
    }
}
