using FluentResults;

namespace AircraftTracker.Errors;
internal static class ParserError
{
    public static Error TableNotFound { get => new("Unable to find flight Table"); }
    public static Error NoFlightsFound { get => new("No Flights were found in table"); }
    public static Error GeneralError { get => new("A Parsing Error occured parsing flightaware.com"); }
}
