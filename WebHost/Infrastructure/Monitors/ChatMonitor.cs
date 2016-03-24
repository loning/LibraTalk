using System;
using System.Collections;
using System.Collections.Generic;
using WebHost.Infrastructure.Actions;
namespace WebHost.Infrastructure.Monitors
{
    public class ChatMonitor : IChatMonitor
    {
        private readonly IList<IObserver<IChatMessageAction>> observers;

        public ChatMonitor()
        {
            observers = new List<IObserver<IChatMessageAction>>();
        }

        public void TrackAction(IChatMessageAction action)
        {
            lock (((ICollection)observers).SyncRoot)
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(action);
                }
            }
        }

        public void Shutdown()
        {
            lock (((ICollection)observers).SyncRoot)
            {
                foreach (var observer in observers)
                {
                    observer.OnCompleted();
                }
            }
        }

        public IDisposable Subscribe(IObserver<IChatMessageAction> observer)
        {
            if (null == observer)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (((ICollection) observers).SyncRoot)
            {
                if (observers.Contains(observer))
                {
                    throw new InvalidOperationException();
                }

                observers.Add(observer);
            }

            return new SubscriptionToken(this, observer);
        }

        public void Unsubscribe(IObserver<IChatMessageAction> observer)
        {
            if (null == observer)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (((ICollection)observers).SyncRoot)
            {
                if (!observers.Contains(observer))
                {
                    throw new InvalidOperationException();
                }

                observers.Remove(observer);
            }
        }

        private class SubscriptionToken : IDisposable
        {
            private readonly ChatMonitor monitor;
            private readonly IObserver<IChatMessageAction> observer;

            public SubscriptionToken(ChatMonitor monitor, IObserver<IChatMessageAction> observer)
            {
                this.monitor = monitor;
                this.observer = observer;
            }

            public void Dispose()
            {
                monitor.Unsubscribe(observer);
            }
        }
    }
}