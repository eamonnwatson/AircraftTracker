using AircraftTracker.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;
using Microsoft.Extensions.Configuration;

namespace AircraftTracker.Pushover;
internal static class PushoverExtensions
{
    public static IServiceCollection AddPushover(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

#if DEBUG
        return services.AddSingleton<IPushoverClient, NullPushoverClient>();
#else
        return services.AddPushCore();
#endif
    }

    private static IServiceCollection AddPushCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions();
        services.AddOptions<PushoverOptions>()
            .Configure<IServiceProvider>((options, provider) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.APIToken = configuration.GetValue("Pushover_APIToken", string.Empty)!;
                options.UserKey = configuration.GetValue("Pushover_UserKey", string.Empty)!;
            });

        services.AddHttpClient<IPushoverClient, PushoverClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.pushover.net/1/messages.json");
        }).AddPolicyHandler(RetryPolicy);

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> RetryPolicy => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
