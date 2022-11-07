using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Kralizek.Lambda;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SqsEventHandlerFunction;

public class SqsMessageHandler : IMessageHandler<Entry>
{
    private readonly IItemProcessor _processor;
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly SqsMessageHandlerOptions _options;
    private readonly ILogger<SqsMessageHandler> _logger;

    public SqsMessageHandler(
        IItemProcessor processor,
        IAmazonSimpleNotificationService sns,
        IOptions<SqsMessageHandlerOptions> options,
        ILogger<SqsMessageHandler> logger)
    {
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _sns = sns ?? throw new ArgumentNullException(nameof(sns));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(Entry? message, ILambdaContext context)
    {
        if (message is null) return;

        _logger.LogDebug("Processing item {Id}", message.Id);

        await _processor.ProcessItem(message.Id);

        var notification = JsonSerializer.Serialize(new NotificationMessage(message.Id, true));

        await _sns.PublishAsync(_options.TopicArn, notification);
    }
}

public record Entry(Guid Id);

public record SqsMessageHandlerOptions
{
    public string TopicArn { get; init; } = default!;
}

public record NotificationMessage (Guid Id, bool IsSuccess);

public interface IItemProcessor
{
    Task ProcessItem(Guid itemId);
}

public class NullItemProcessor : IItemProcessor
{
    public Task ProcessItem(Guid itemId) => Task.CompletedTask;
}