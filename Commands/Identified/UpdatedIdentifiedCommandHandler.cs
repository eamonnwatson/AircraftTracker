using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.Identified;
internal class UpdatedIdentifiedCommandHandler(IRepository repository) : IRequestHandler<UpdatedIdentifiedCommand>
{
    private readonly IRepository _repository = repository;

    public Task Handle(UpdatedIdentifiedCommand request, CancellationToken cancellationToken)
    {
        _repository.UpdateIdentified(request.IdentifiedFlights);
        return Task.CompletedTask;
    }
}
