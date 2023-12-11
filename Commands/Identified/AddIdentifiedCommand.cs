using MediatR;

namespace AircraftTracker.Commands.Identified;
internal record AddIdentifiedCommand(IEnumerable<Entities.Identified> Identified) : IRequest;
