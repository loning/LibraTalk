using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Awaitable
{
    public static class EventAsync
    {
        public static Task FromEvent(
            Action<EventHandler> subscribe, 
            Action<EventHandler> cleanup,
            Action action = null)
        {
            return new EventHandlerTaskSource(subscribe, cleanup, action).Task;
        }

        public static Task<object> FromEvent<T>(
            Action<EventHandler<T>> subscribe, 
            Action<EventHandler<T>> cleanup,
            Action action = null)
        {
            return new EventHandlerTaskSource<T>(subscribe, cleanup, action).Task;
        }

        /// <summary>
        /// Creates the <see cref="Task" /> that waits for an event to occur.
        /// </summary>
        /// <param name="subscribe">The <see cref="Action" /> to subscribe to event.</param>
        /// <param name="cleanup"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task<RoutedEventArgs> FromRoutedEvent(
            Action<RoutedEventHandler> subscribe,
            Action<RoutedEventHandler> cleanup,
            Action action = null)
        {
            return new RoutedEventHandlerTaskSource(subscribe, cleanup, action).Task;
        }

        private sealed class EventHandlerTaskSource
        {
            private readonly TaskCompletionSource<bool> tcs;
            private readonly Action<EventHandler> cleanup;

            public Task Task => tcs.Task;

            public EventHandlerTaskSource(
                Action<EventHandler> subscribe,
                Action<EventHandler> cleanup, 
                Action action)
            {
                if (null == subscribe)
                {
                    throw new ArgumentNullException(nameof(subscribe));
                }

                if (null == cleanup)
                {
	                throw new ArgumentNullException(nameof(cleanup));
                }

                tcs = new TaskCompletionSource<bool>();
                this.cleanup = cleanup;

                subscribe.Invoke(DoComplete);
                action?.Invoke();
            }

            private void DoComplete(object sender, EventArgs e)
            {
                cleanup.Invoke(DoComplete);
                tcs.SetResult(true);
            }
        }

        private sealed class EventHandlerTaskSource<TEventArgs>
        {
            private readonly TaskCompletionSource<object> tcs;
            private readonly Action<EventHandler<TEventArgs>> cleanup;

            public Task<object> Task => tcs.Task;

            public EventHandlerTaskSource(
                Action<EventHandler<TEventArgs>> subscribe,
                Action<EventHandler<TEventArgs>> cleanup, 
                Action action)
            {
                if (null == subscribe)
                {
                    throw new Exception();
                }

                if (null == cleanup)
                {
                    throw new Exception();
                }

                tcs = new TaskCompletionSource<object>();
                this.cleanup = cleanup;

                subscribe.Invoke(DoComplete);
                action?.Invoke();
            }

            private void DoComplete(object sender, TEventArgs e)
            {
                cleanup.Invoke(DoComplete);
                tcs.SetResult(e);
            }
        }

        private sealed class RoutedEventHandlerTaskSource
        {
            private readonly TaskCompletionSource<RoutedEventArgs> tcs;
            private readonly Action<RoutedEventHandler> cleanup;

            public Task<RoutedEventArgs> Task => tcs.Task;

            public RoutedEventHandlerTaskSource(
                Action<RoutedEventHandler> subscribe,
                Action<RoutedEventHandler> cleanup, 
                Action action)
            {
                if (null == subscribe)
                {
                    throw new Exception();
                }

                if (null == cleanup)
                {
                    throw new Exception();
                }

                tcs = new TaskCompletionSource<RoutedEventArgs>();
                this.cleanup = cleanup;

                subscribe.Invoke(DoComplete);
                action?.Invoke();
            }

            private void DoComplete(object sender, RoutedEventArgs e)
            {
                cleanup.Invoke(DoComplete);
                tcs.SetResult(e);
            }
        }
    }
}