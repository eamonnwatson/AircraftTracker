using FluentResults;

namespace AircraftTracker.Errors;
internal static class DatabaseErrors
{
    public static Error Alerts { get => new("An Error occured retrieving Alert Types"); }
    public static Error Identified { get => new("An Error occured retrieving Identified Types"); }
    public static Error LiveFlights { get => new("An Error occured retrieving Live Flights"); }
}
