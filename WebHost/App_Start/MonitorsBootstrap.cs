using WebHost.Infrastructure;

namespace WebHost
{
    public static class MonitorsBootstrap
    {
        public static void Register()
        {
            new ChatPollTask().SubscribeTo(ActionMonitors.Chat);
        }
    }
}