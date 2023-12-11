using LiteDB;

namespace AircraftTracker.LiteDB;
internal class LiteDatabaseOptions
{
    public ConnectionString ConnectionString { get; set; } = new ConnectionString();

    public void SetConnectionString(string connectionString)
    {
        ConnectionString = new ConnectionString(connectionString);
    }

}
