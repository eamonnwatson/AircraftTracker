using LiteDB;

namespace AircraftTracker.LiteDB;
internal interface ILiteDatabaseFactory
{
    LiteDatabase Create();
}
