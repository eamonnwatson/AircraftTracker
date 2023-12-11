using Microsoft.Extensions.Options;

namespace AircraftTracker.Options;
internal class CheckerOptions 
{
    public string FlightAwareURL { get; set; } = default!;
    public string EnrouteTableNode { get; set; } = default!;
    public int FrequencyOfCheckSeconds { get; set; } = 300;
}
