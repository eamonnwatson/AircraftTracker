using AircraftTracker.Interfaces;
using MediatR;

namespace AircraftTracker.Commands.LiveFlights;
internal class SendNewFlightAlertCommandHandler(IPushoverClient pushoverClient) : IRequestHandler<SendNewFlightAlertCommand>
{
    private readonly IPushoverClient _pushoverClient = pushoverClient;
    private const string AlertNotificationMessage = "The following new flights have been found with new aircraft types : \n\n{0}";
    public async Task Handle(SendNewFlightAlertCommand request, CancellationToken cancellationToken)
    {
        if (!request.Flights.Any())
            return;

        var flights = request.Flights.Select(f => $"{f.Ident} ({f.Type}/{f.FullType}) - {f.From} - {f.Arrive}");
        var message = string.Format(AlertNotificationMessage, flights);

        await _pushoverClient.PushAsync("New Flight Notification", message);
    }
}
