using LiteDB;

namespace AircraftTracker.Entities;
internal class LiveFlight
{
    public ObjectId LiveFlightId { get; }
    public string Ident { get; }
    public string Link { get; }
    public string Airline { get; }
    public string Type { get; }
    public string FullType { get; }
    public string From { get; }
    public string Depart { get; }
    public string Arrive { get; }
    public DateTime InsertedDate { get; }

    public static LiveFlight Create(string ident, string link, string airline, string type, string fullType, string from, 
                                    string depart, string arrive)
    {
        return new LiveFlight(ObjectId.NewObjectId(), ident, link, airline, type, fullType, from, depart, arrive, DateTime.UtcNow);
    }

    [BsonCtor]
    public LiveFlight(ObjectId liveFlightId, string ident, string link, string airline, string type, string fullType, 
                       string from, string depart, string arrive, DateTime insertedDate)
    {
        LiveFlightId = liveFlightId;
        Ident = ident;
        Link = link;
        Airline = airline;
        Type = type;
        FullType = fullType;
        From = from;
        Depart = depart;
        Arrive = arrive;
        InsertedDate = insertedDate;
    } 

}
    

