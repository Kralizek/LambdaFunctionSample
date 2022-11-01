using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FindNationality;

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
      logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
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

public class FindNationalityHandler : IRequestResponseHandler<string, Country[]>
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ILogger<FindNationalityHandler> _logger;
  private readonly FindNationalityOptions _options;

  public FindNationalityHandler(IHttpClientFactory httpClientFactory, IOptions<FindNationalityOptions> options, ILogger<FindNationalityHandler> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
  }

  public async Task<Country[]> HandleAsync(string? input, ILambdaContext context)
  {
    var http = _httpClientFactory.CreateClient(nameof(FindNationalityHandler));

    _logger.LogDebug("Finding the nationality of people named {Name}", input);

    var response = await http.GetFromJsonAsync<Response>($"https://api.nationalize.io/?name={input}");

    return response?.Countries.Where(c => c.Probability >= _options.MinimumThreshold).ToArray() ?? Array.Empty<Country>();
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