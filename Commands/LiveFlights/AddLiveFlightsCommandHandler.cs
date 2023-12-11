using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal class AddLiveFlightsCommandHandler(IRepository repository) : IRequestHandler<AddLiveFlightsCommand>
{
    private readonly IRepository _repository = repository;

    public Task Handle(AddLiveFlightsCommand request, CancellationToken cancellationToken)
    {
        _repository.AddLiveFlights(request.Flights);
        return Task.CompletedTask;
    }
}
