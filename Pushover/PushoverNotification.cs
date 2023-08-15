using AircraftTracker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTracker.Pushover;
internal class PushoverNotification : INotificationService
{
    private readonly ILogger<PushoverNotification> logger;
    private readonly IPushoverClient client;

    public PushoverNotification(IPushoverClient client, ILogger<PushoverNotification> logger)
    {
        this.client = client;
        this.logger = logger;
    }
    public async Task SendNotificationAsync(string text, CancellationToken cancellationToken)
    {
        await client.PushAsync("New Flight Notification", text);

    }
}
