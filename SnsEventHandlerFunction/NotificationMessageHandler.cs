using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Logging;

namespace SnsEventHandlerFunction;

public class NotificationMessageHandler : INotificationHandler<NotificationMessage>
{
    public Task HandleAsync(NotificationMessage? notification, ILambdaContext context)
    {
        throw new NotImplementedException();
    }
}