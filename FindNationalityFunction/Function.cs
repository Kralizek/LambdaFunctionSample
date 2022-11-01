using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FindNationality;

public class Function : RequestResponseFunction<string, Country[]>
{
  protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
  {
    RegisterHandler<FindNationalityHandler>(services);

    // Add registration for HttpClient
    services.AddHttpClient("Nationality");
  }
}


public class FindNationalityHandler : IRequestResponseHandler<string, Country[]>
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ILogger<FindNationalityHandler> _logger;

  public FindNationalityHandler(IHttpClientFactory httpClientFactory, ILogger<FindNationalityHandler> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
  }

  public async Task<Country[]> HandleAsync(string? input, ILambdaContext context)
  {
    var http = _httpClientFactory.CreateClient("Nationality");

    _logger.LogDebug("Finding the nationality of people named {Name}", input);

    var response = await http.GetFromJsonAsync<Response>($"https://api.nationalize.io/?name={input}");

    return response?.Countries ?? Array.Empty<Country>();
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