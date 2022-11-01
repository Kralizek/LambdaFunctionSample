using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Kralizek.Lambda;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FindNationalityFunctionV2;

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
    var http = _httpClientFactory.CreateClient("Nationality");

    _logger.LogDebug("Finding the nationality of people named {Name}", input);

    var response = await http.GetFromJsonAsync<Response>($"https://api.nationalize.io/?name={input}");

    return response?.Countries.Where(c => c.Probability >= _options.MinimumThreshold).ToArray() ?? Array.Empty<Country>();
  }
}
