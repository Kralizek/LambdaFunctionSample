using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SnsEventHandlerFunction;

public class Function : EventFunction<SNSEvent>
{
    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        services.UseNotificationHandler<NotificationMessage, NotificationMessageHandler>();
    }
}
