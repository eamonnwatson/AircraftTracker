using AircraftTracker.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AircraftTracker;
internal class EmailNotification : INotificationService
{
    private readonly ILogger<EmailNotification> logger;
    private readonly IConfiguration configuration;
    private readonly IFluentEmailFactory emailFactory;

    public EmailNotification(ILogger<EmailNotification> logger, IConfiguration configuration, IFluentEmailFactory emailFactory)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.emailFactory = emailFactory;
    }
    public async Task SendNotificationAsync(string text, CancellationToken cancellationToken)
    {
        var email = emailFactory.Create();

        await email
            .To(configuration.GetValue("EMAIL_TO", string.Empty))
            .Subject("New Flight Notification")
            .Body(text)
            .SendAsync(cancellationToken);

    }
}
