using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.Identified;
internal class AddIdentifiedCommandHandler(IRepository repository) : IRequestHandler<AddIdentifiedCommand>
{
    private readonly IRepository _repository = repository;

    public Task Handle(AddIdentifiedCommand request, CancellationToken cancellationToken)
    {
        _repository.AddIdentified(request.Identified);
        return Task.CompletedTask;
    }
}
