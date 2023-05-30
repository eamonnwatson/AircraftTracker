namespace AircraftTracker.Interfaces;
internal interface INotificationService
{
    Task SendNotificationAsync(string text, CancellationToken cancellationToken);
}
