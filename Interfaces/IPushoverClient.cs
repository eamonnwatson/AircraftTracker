using AircraftTracker.Pushover;

namespace AircraftTracker.Interfaces;
internal interface IPushoverClient
{
    Task<PushResponse> PushAsync(string title, string message);
}
