using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AircraftTracker.LiteDB;
internal static class LiteDatabaseExtensions
{
    public const string LiteDBConnectionStringKey = "LiteDatabase";
    public static IServiceCollection AddLiteDatabase(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddLiteDB();

    }

    public static IServiceCollection AddLiteDatabase(this IServiceCollection services, Action<LiteDatabaseOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        return services.AddLiteDB().Configure(configure);
    }

    public static IServiceCollection AddLiteDatabase(this IServiceCollection services, LiteDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        return services.AddLiteDatabase(cfg =>
        {
            cfg.ConnectionString = options.ConnectionString;
        });
    }

    private static IServiceCollection AddLiteDB(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions();
        services.AddOptions<LiteDatabaseOptions>()
            .Configure<IServiceProvider>((options, provider) =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString(LiteDBConnectionStringKey);
                if (connectionString is not null)
                    options.SetConnectionString(connectionString);
            });

        services.TryAddTransient<ILiteDatabaseFactory, LiteDatabaseFactory>();
        services.TryAddSingleton<ILiteDatabase>(provider =>
        {
            var factory = provider.GetRequiredService<ILiteDatabaseFactory>();
            return factory.Create();
        });

        return services;
    }
}

