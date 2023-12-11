using LiteDB;
using Microsoft.Extensions.Options;

namespace AircraftTracker.LiteDB;
internal class LiteDatabaseFactory(IOptions<LiteDatabaseOptions> options) : ILiteDatabaseFactory
{
    private readonly LiteDatabaseOptions _options = options.Value;

    public LiteDatabase Create()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString.Filename))
            throw new ArgumentNullException("Connection string invalid", nameof(ConnectionString.Filename));

        return new LiteDatabase(_options.ConnectionString);
    }
}
