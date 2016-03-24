using WebHost.Infrastructure.Monitors;

namespace WebHost.Infrastructure
{
    public class ActionMonitors
    {
        public static readonly IChatMonitor Chat;

        static ActionMonitors()
        {
            Chat = new ChatMonitor();
        }
    }
}