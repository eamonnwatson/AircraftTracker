using AircraftTracker.Entities;
using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.LiveFlights;
internal record RequestLiveFlightsFromDatabase : IRequest<Result<IEnumerable<LiveFlight>>>;
