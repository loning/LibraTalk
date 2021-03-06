﻿using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans.Streams;
using WebHost.Infrastructure.Actions;

namespace WebHost.Infrastructure.Monitors
{
    public interface IMessagesMonitor : IObservable<IChatMessageAction>, IAsyncObserver<RoomMessage>
    {
        Task StartTracking();

        Task Shutdown();
    }
}