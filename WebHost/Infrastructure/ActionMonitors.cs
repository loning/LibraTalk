using WebHost.Infrastructure.Monitors;

namespace WebHost.Infrastructure
{
    public class ActionMonitors
    {
        public static readonly IMessagesMonitor Messages;

        static ActionMonitors()
        {
            Messages = new MessagesMonitor();
        }
    }
}