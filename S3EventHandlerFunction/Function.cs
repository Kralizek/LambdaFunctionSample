using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
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
        // Use this method to register your configuration flow. Exactly like in ASP.NET Core
    }

    protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment)
    {
        // Use this method to configure the logging
    }

    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        RegisterHandler<S3EventHandler>(services);
    }
}
