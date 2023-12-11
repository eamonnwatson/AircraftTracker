using AircraftTracker.Entities;
using FluentResults;
using LiteDB;

namespace AircraftTracker.Interfaces;
internal interface IRepository
{
    Result<IEnumerable<Alert>> GetFlightAlerts();
    Result<IEnumerable<Identified>> GetIdentified();
    Result<IEnumerable<LiveFlight>> GetLiveFlights();

    void AddIdentified(IEnumerable<Identified> identified);
    void UpdateAlerts(IEnumerable<Alert> alerts);
    void UpdateIdentified(IEnumerable<Identified> identified);
    void DeleteLiveFlights(IEnumerable<LiveFlight> liveFlights);
    void AddLiveFlights(IEnumerable<LiveFlight> liveFlights);
}
