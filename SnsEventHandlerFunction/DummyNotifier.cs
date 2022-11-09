using System.Threading.Tasks;

namespace SnsEventHandlerFunction;

public class DummyNotifier : INotifier
{
    public Task NotifyRecipients(NotificationMessage message) => Task.CompletedTask;
}