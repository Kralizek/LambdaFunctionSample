using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FindNationalityFunctionV2;

public class Function : RequestResponseFunction<string, Country[]>
{
    protected override void Configure(IConfigurationBuilder builder)
    {
        // Use this method to register your configuration flow. Exactly like in ASP.NET Core
    }

    protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment)
    {
        // Use this method to install logger providers
    }

    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        // You need this line to register your handler
        RegisterHandler<FindNationalityHandler>(services);

        // Use this method to register your services. Exactly like in ASP.NET Core
    }
}

public record Response(
    [property: JsonPropertyName("country")] Country[] Countries,
    [property: JsonPropertyName("name")] string Name
);

public record Country(
    [property: JsonPropertyName("country_id")] string CountryCode,
    [property: JsonPropertyName("probability")] double Probability
);

public record FindNationalityOptions
{
  public double MinimumThreshold { get; init; }
}