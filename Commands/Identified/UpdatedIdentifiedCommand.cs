using MediatR;

namespace AircraftTracker.Commands.Identified;
internal record UpdatedIdentifiedCommand(IEnumerable<Entities.Identified> IdentifiedFlights) : IRequest;
