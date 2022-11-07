using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleNotificationService;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SqsEventHandlerFunction;

public class Function : EventFunction<SQSEvent>
{
    protected override void Configure(IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Options:TopicArn"] = "ARN of my SNS topic"
        });
    }

    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        services.UseQueueMessageHandler<Entry, SqsMessageHandler>().WithParallelExecution();

        services.AddAWSService<IAmazonSimpleNotificationService>();

        services.Configure<SqsMessageHandlerOptions>(Configuration.GetSection("Options"));

        services.UseCustomMessageSerializer<CustomSerializer>();

        services.AddSingleton<IItemProcessor, NullItemProcessor>();
    }
}
