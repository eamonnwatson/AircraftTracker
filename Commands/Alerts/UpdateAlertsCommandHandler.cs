using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.Alerts;
internal class UpdateAlertsCommandHandler(IRepository repository) : IRequestHandler<UpdateAlertsCommand>
{
    private readonly IRepository _repository = repository;

    public Task Handle(UpdateAlertsCommand request, CancellationToken cancellationToken)
    {
        _repository.UpdateAlerts(request.Alerts);
        return Task.CompletedTask;
    }
}
