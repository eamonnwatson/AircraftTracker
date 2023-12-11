using AircraftTracker.Entities;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.FlightAware;
internal record RequestLiveFlightsFromWeb() : IRequest<Result<IEnumerable<LiveFlight>>>;
