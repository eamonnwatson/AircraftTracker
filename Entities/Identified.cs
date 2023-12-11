using LiteDB;

namespace AircraftTracker.Entities;
internal class Identified
{
    public ObjectId IdentifiedId { get; }
    public string AircraftType { get; }
    public DateTime LastViewed { get; set; }

    public static Identified Create(string aircraftType)
    {
        return new Identified(ObjectId.NewObjectId(), aircraftType, DateTime.UtcNow);
    }

    [BsonCtor]
    public Identified(ObjectId identifiedId, string aircraftType, DateTime lastViewed)
    {
        IdentifiedId = identifiedId;
        AircraftType = aircraftType;
        LastViewed = lastViewed;
    }

}
