using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.SQS;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3EventHandlerFunction;

public class Function : EventFunction<S3Event>
{
    protected override void Configure(IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Options:QueueUrl"] = "url to my SQS queue"
        });
    }

    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        RegisterHandler<S3EventHandler>(services);

        services.AddAWSService<IAmazonS3>();

        services.AddAWSService<IAmazonSQS>();

        services.Configure<S3EventHandlerOptions>(Configuration.GetSection("Options"));
    }
}
