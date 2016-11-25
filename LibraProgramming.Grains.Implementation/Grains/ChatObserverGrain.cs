using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Streams;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatObserverGrain : Grain, IChatObserver
    {
        private IAsyncStream<ChatMessage> messages;
        private IList<StreamSubscriptionHandle<ChatMessage>> subscriptions;
        private TaskCompletionSource<ChatMessage> tcs;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        async Task IChatObserver.SubscribeAsync()
        {
            var subscription = await messages.SubscribeAsync(OnNextMessage);
            subscriptions.Add(subscription);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        async Task<ChatMessage> IChatObserver.WaitForUpdates(TimeSpan timeout)
        {
            var tasks = new[] { tcs.Task, Task.Delay(timeout) };
            var completed = await Task.WhenAny(tasks);

            if (completed == tcs.Task && tcs.Task.IsCompleted)
            {
                return tcs.Task.Result;
            }

            return null;
        }

        public override async Task OnActivateAsync()
        {
            var provider = GrainClient.GetStreamProvider(Configuration.StreamProviderName);

            tcs = new TaskCompletionSource<ChatMessage>();
            messages = provider.GetStream<ChatMessage>(this.GetPrimaryKey(), Configuration.Namespace);
            subscriptions = new List<StreamSubscriptionHandle<ChatMessage>>();

            foreach (var handle in await messages.GetAllSubscriptionHandles())
            {
                var subscription = await handle.ResumeAsync(OnNextMessage);
                subscriptions.Add(subscription);
            }

            await base.OnActivateAsync();
        }

        private Task OnNextMessage(ChatMessage message, StreamSequenceToken sst)
        {
            tcs.TrySetResult(message);
            return TaskDone.Done;
        }
    }
}