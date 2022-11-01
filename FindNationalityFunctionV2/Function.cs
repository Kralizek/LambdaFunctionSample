using System.Collections.Generic;
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
    protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
    {
        RegisterHandler<FindNationalityHandler>(services);

        // Add registration for HttpClient
        services.AddHttpClient("Nationality");

        services.AddOptions();

        services.Configure<FindNationalityOptions>(Configuration.GetSection("Options"));
    }

    protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment)
    {
        logging.AddLambdaLogger(new LambdaLoggerOptions
        {
            IncludeCategory = true,
            IncludeLogLevel = true,
            IncludeNewline = true,
        });

        if (executionEnvironment.IsProduction())
        {
            logging.SetMinimumLevel(LogLevel.Warning);
        }
    }

    protected override void Configure(IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Options:MinimumThreshold"] = "0.05"
        });
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