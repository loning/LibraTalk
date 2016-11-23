using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;
using Orleans.Streams;

namespace LibraProgramming.Web.Services.Core
{
    internal sealed class ChatObserver : IAsyncObserver<ChatMessage>
    {
        private readonly TaskCompletionSource<ChatMessage> tcs;
        private StreamSubscriptionHandle<ChatMessage> subscription;

        public ChatObserver()
        {
            tcs = new TaskCompletionSource<ChatMessage>();
        }

        public async Task SubscribeAsync(IAsyncStream<ChatMessage> stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            subscription = await stream.SubscribeAsync(this);
        }

        public async Task<IReadOnlyCollection<ChatMessage>> WaitForUpdates(TimeSpan delay, CancellationToken token)
        {
            var messages = new List<ChatMessage>();
            var task = tcs.Task;

            await Task.WhenAny(task, Task.Delay(delay, token));

            if (task.IsCompleted)
            {
                messages.Add(task.Result);
            }

            return new ReadOnlyCollection<ChatMessage>(messages);
        }

        public Task ReleaseAsync()
        {
            var temp = subscription;
            subscription = null;
            return temp.UnsubscribeAsync();
        }

        Task IAsyncObserver<ChatMessage>.OnNextAsync(ChatMessage item, StreamSequenceToken token = null)
        {
            tcs.TrySetResult(item);
            return TaskDone.Done;
        }

        Task IAsyncObserver<ChatMessage>.OnCompletedAsync()
        {
            tcs.TrySetCanceled();
            return TaskDone.Done;
        }

        Task IAsyncObserver<ChatMessage>.OnErrorAsync(Exception exception)
        {
            tcs.TrySetException(exception);
            return TaskDone.Done;
        }
    }
}