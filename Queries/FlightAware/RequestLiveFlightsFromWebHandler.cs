using AircraftTracker.Entities;
using AircraftTracker.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Registry;

namespace AircraftTracker.Queries.FlightAware;
internal class RequestLiveFlightsFromWebHandler(ResiliencePipelineProvider<string> pipeline,
                                         IFlightParser parser,
                                         IConfiguration configuration) : IRequestHandler<RequestLiveFlightsFromWeb, Result<IEnumerable<LiveFlight>>>
{
    private readonly ResiliencePipeline<Result<IEnumerable<LiveFlight>>> _pipeline = pipeline.GetPipeline<Result<IEnumerable<LiveFlight>>>("flight-pipeline");
    private readonly IFlightParser _parser = parser;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<IEnumerable<LiveFlight>>> Handle(RequestLiveFlightsFromWeb request, CancellationToken cancellationToken)
    {
        var airport = _configuration.GetValue<string>("airport")!;

        var result = await _pipeline.ExecuteAsync(async (token) => await _parser.GetFlightsAsync(airport, token), cancellationToken);

        return result;
    }
}
