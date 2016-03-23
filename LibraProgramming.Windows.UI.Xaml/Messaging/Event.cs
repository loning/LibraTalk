using System;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public enum ExecutionMode
    {
        Synchronously,
        BackgroundTask,
        UIThread
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class Event<TPayload> : EventBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="option"></param>
        /// <param name="keepSubscriberAlive"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(
            Action<TPayload> action,
            ExecutionMode option = ExecutionMode.Synchronously,
            bool keepSubscriberAlive = true,
            Predicate<TPayload> filter = null)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionReference = CreateActionReference(action, keepSubscriberAlive);
            var filterReference = CreateFilterReference(filter, keepSubscriberAlive);

            IEventSubscription subscription = null;

            switch (option)
            {
                case ExecutionMode.Synchronously:
                    subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                    break;

                case ExecutionMode.BackgroundTask:
                    subscription = new BackgroundTaskEventSubscription<TPayload>(actionReference, filterReference);
                    break;

                case ExecutionMode.UIThread:
                    subscription = new DispatcherEventSubscription<TPayload>(actionReference, filterReference,
                        SynchronizationContext);
                    break;
            }

            return SubscribeInternal(subscription);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Task PublishAsync(TPayload payload)
        {
            return PublishInternalAsync(payload);
        }

        private static IActionReference<TPayload> CreateActionReference(Action<TPayload> action, bool keepAlive)
        {
            if (keepAlive)
            {
                return new StrongActionReference<TPayload>(action);
            }

            return new WeakActionReference<TPayload>(action);
        }

        private static IPredicateReference<TPayload> CreateFilterReference(Predicate<TPayload> filter, bool keepSubscriberAlive)
        {
            var predicate = filter ?? AcceptAny;

            if (keepSubscriberAlive)
            {
                return new StrongPredicateReference<TPayload>(predicate);
            }

            return new WeakPredicateReference<TPayload>(predicate);
        }

        private static bool AcceptAny(TPayload payload)
        {
            return true;
        }
    }
}