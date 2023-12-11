using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal class DeleteLiveFlightsCommandHandler(IRepository repository) : IRequestHandler<DeleteLiveFlightsCommand>
{
    private readonly IRepository _repository = repository;

    public Task Handle(DeleteLiveFlightsCommand request, CancellationToken cancellationToken)
    {
        _repository.DeleteLiveFlights(request.Flights);
        return Task.CompletedTask;
    }
}
