using AircraftTracker.Entities;
using AircraftTracker.Errors;
using AircraftTracker.Interfaces;
using FluentResults;
using LiteDB;

namespace AircraftTracker;
internal class Repository(ILiteDatabase database) : IRepository
{
    private readonly ILiteDatabase _database = database;
    public void AddLiveFlights(IEnumerable<LiveFlight> liveFlights)
    {
        _database.GetCollection<LiveFlight>().InsertBulk(liveFlights);
    }

    public void DeleteLiveFlights(IEnumerable<LiveFlight> liveFlights)
    {
        var col = _database.GetCollection<LiveFlight>();

        foreach (var liveFlight in liveFlights)
        {
            col.Delete(liveFlight.LiveFlightId);
        }
    }

    public Result<IEnumerable<Alert>> GetFlightAlerts()
    {
        try
        {
            return _database.GetCollection<Alert>().FindAll().ToList();
        }
        catch (LiteException ex)
        {
            return DatabaseErrors.Alerts.CausedBy(ex);
        }
    }

    public Result<IEnumerable<Identified>> GetIdentified()
    {
        try
        {
            return _database.GetCollection<Identified>().FindAll().ToList();
        }
        catch (LiteException ex)
        {
            return DatabaseErrors.Identified.CausedBy(ex);
        }
    }

    public Result<IEnumerable<LiveFlight>> GetLiveFlights()
    {
        try
        {
            return _database.GetCollection<LiveFlight>().FindAll().ToList();
        }
        catch (LiteException ex)
        {
            return DatabaseErrors.LiveFlights.CausedBy(ex);
        }
    }

    public void UpdateAlerts(IEnumerable<Alert> alerts)
    {
        _database.GetCollection<Alert>().Update(alerts);
    }

    public void UpdateIdentified(IEnumerable<Identified> identified)
    {
        _database.GetCollection<Identified>().Update(identified);
    }
    public void AddIdentified(IEnumerable<Identified> identified)
    {
        _database.GetCollection<Identified>().InsertBulk(identified);
    }
}
