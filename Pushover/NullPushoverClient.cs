using AircraftTracker.Interfaces;

namespace AircraftTracker.Pushover;
internal class NullPushoverClient : IPushoverClient
{
    public Task<PushResponse> PushAsync(string title, string message)
    {
        return Task.FromResult(new PushResponse());
    }
}
