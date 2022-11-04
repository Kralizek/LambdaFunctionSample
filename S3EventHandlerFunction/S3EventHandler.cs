using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Kralizek.Lambda;

namespace S3EventHandlerFunction;

public class S3EventHandler : IEventHandler<S3Event>
{
    public Task HandleAsync(S3Event? input, ILambdaContext context)
    {
        throw new System.NotImplementedException();
    }
}
