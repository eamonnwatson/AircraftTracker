using FluentResults;
using MediatR;

namespace AircraftTracker.Queries.Identified;
internal record RequestIdentifiedFromDatabase() : IRequest<Result<IEnumerable<Entities.Identified>>>;
