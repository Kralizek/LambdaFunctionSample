using System;
using Kralizek.Lambda;

namespace SqsEventHandlerFunction;

public sealed class CustomSerializer : IMessageSerializer
{
    public TMessage? Deserialize<TMessage>(string input)
    {
        if (typeof(TMessage) != typeof(Entry))
        {
            throw new NotSupportedException();
        }

        if (!Guid.TryParse(input, out var id))
        {
            throw new FormatException("Format not valid");
        }

        return (TMessage)(object)new Entry(id);
    }
}