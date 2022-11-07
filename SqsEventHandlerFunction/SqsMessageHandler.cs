using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Kralizek.Lambda;

namespace SqsEventHandlerFunction;

public class SqsMessageHandler : IMessageHandler<Entry>
{
    public Task HandleAsync(Entry? message, ILambdaContext context)
    {
        throw new NotImplementedException();
    }
}

public record Entry(Guid Id);
