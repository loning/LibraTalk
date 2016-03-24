using System;
using WebHost.Infrastructure.Actions;

namespace WebHost.Infrastructure.Monitors
{
    public interface IChatMonitor : IObservable<IChatMessageAction>
    {
        void TrackAction(IChatMessageAction action);

        void Shutdown();
    }
}