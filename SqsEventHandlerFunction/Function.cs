using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SqsEventHandlerFunction;

public class Function : EventFunction<SQSEvent>
{
    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        services.UseQueueMessageHandler<Entry, SqsMessageHandler>();
    }
}
