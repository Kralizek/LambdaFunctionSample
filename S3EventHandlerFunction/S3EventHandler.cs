using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Kralizek.Lambda;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace S3EventHandlerFunction;

public class S3EventHandler : IEventHandler<S3Event>
{
    private readonly IAmazonS3 _s3;
    private readonly IAmazonSQS _sqs;
    private readonly S3EventHandlerOptions _options;
    private readonly ILogger<S3EventHandler> _logger;

    public S3EventHandler(IAmazonS3 s3, IAmazonSQS sqs, IOptions<S3EventHandlerOptions> options, ILogger<S3EventHandler> logger)
    {
        _s3 = s3 ?? throw new ArgumentNullException(nameof(s3));
        _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(S3Event? input, ILambdaContext context)
    {
        if (input is not { Records.Count: > 0 }) return;

        foreach (var record in input.Records)
        {
            string key = record.S3.Object.Key;
            string bucket = record.S3.Bucket.Name;

            _logger.LogDebug("Processing {Key} in {Bucket}", key, bucket);

            var response = await _s3.GetObjectAsync(bucket, key);

            var entries = JsonSerializer.Deserialize<Entry[]>(response.ResponseStream);

            if (entries is null) continue;

            foreach (var entry in entries)
            {
                await _sqs.SendMessageAsync(_options.QueueUrl, entry.Id.ToString());
            }
        }
    }
}

public record Entry(Guid Id);

public class S3EventHandlerOptions
{
    public string QueueUrl { get; init; } = default!;
}