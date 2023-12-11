using AircraftTracker.Interfaces;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.Identified;
internal class RequestIdentifiedFromDatabaseHandler(IRepository repository) 
    : IRequestHandler<RequestIdentifiedFromDatabase, Result<IEnumerable<Entities.Identified>>>
{
    private readonly IRepository _repository = repository;

    public Task<Result<IEnumerable<Entities.Identified>>> Handle(RequestIdentifiedFromDatabase request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository.GetIdentified());
    }
}
