using LiteDB;

namespace AircraftTracker.Entities;
internal class Alert
{
    public ObjectId AlertId { get; }
    public string AircraftType { get; }
    public DateTime LastAlert { get; private set; }

    public static Alert Create(string aircraftType)
    {
        return new Alert(ObjectId.NewObjectId(), aircraftType, DateTime.UtcNow);
    }

    public void UpdateAlert()
    {
        LastAlert = DateTime.UtcNow;
    }

    [BsonCtor]
    public Alert(ObjectId alertId, string aircraftType, DateTime lastAlert)
    {
        AlertId = alertId;
        AircraftType = aircraftType;
        LastAlert = lastAlert;
    }
}
