﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Streams;
using WebHost.Infrastructure.Actions;
namespace WebHost.Infrastructure.Monitors
{
    public class ChatMonitor : IChatMonitor
    {
        private StreamSubscriptionHandle<RoomMessage> handle;
        private readonly IList<IObserver<IChatMessageAction>> observers;

        public ChatMonitor()
        {
            observers = new List<IObserver<IChatMessageAction>>();
        }

        async Task IChatMonitor.StartTracking()
        {
            var provider = GrainClient.GetStreamProvider("SMSProvider");
            var room = provider.GetStream<RoomMessage>(Streams.Id, "default");

            handle = await room.SubscribeAsync(this);
        }

        Task IChatMonitor.Shutdown()
        {
            lock (((ICollection)observers).SyncRoot)
            {
                foreach (var observer in observers)
                {
                    observer.OnCompleted();
                }
            }

            return handle.UnsubscribeAsync();
        }

        IDisposable IObservable<IChatMessageAction>.Subscribe(IObserver<IChatMessageAction> observer)
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

        Task IAsyncObserver<RoomMessage>.OnNextAsync(RoomMessage item, StreamSequenceToken token)
        {
            TrackAction(new MessageReceivedChatAction(item));
            return TaskDone.Done;
        }

        Task IAsyncObserver<RoomMessage>.OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        Task IAsyncObserver<RoomMessage>.OnCompletedAsync()
        {
            return ((IChatMonitor) this).Shutdown();
        }

        private void Unsubscribe(IObserver<IChatMessageAction> observer)
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

        private void TrackAction(IChatMessageAction action)
        {
            lock (((ICollection)observers).SyncRoot)
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(action);
                }
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