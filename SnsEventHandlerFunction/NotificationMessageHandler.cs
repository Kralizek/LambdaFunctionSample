using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Logging;

namespace SnsEventHandlerFunction;

public class NotificationMessageHandler : INotificationHandler<NotificationMessage>
{
    private readonly INotifier _notifier;
    private readonly ILogger<NotificationMessageHandler> _logger;

    public NotificationMessageHandler(INotifier notifier, ILogger<NotificationMessageHandler> logger)
    {
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(NotificationMessage? notification, ILambdaContext context)
    {
        if (notification is null) return;

        _logger.LogDebug("Notifying recipients of {Id}", notification.Id);

        await _notifier.NotifyRecipients(notification);
    }
}

public interface INotifier
{
    Task NotifyRecipients(NotificationMessage message);
}
