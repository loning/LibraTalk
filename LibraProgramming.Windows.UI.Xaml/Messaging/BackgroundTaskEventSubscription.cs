using System.Threading.Tasks;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    internal class BackgroundTaskEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        public BackgroundTaskEventSubscription(IActionReference<TPayload> action, IPredicateReference<TPayload> filter)
            : base(action, filter)
        {
        }

        public override Task ExecuteAsync(params object[] args)
        {
            var payload = GetPalyload(args);
            var task = Task.Factory
                .StartNew(() => ExecuteAction(payload), TaskCreationOptions.DenyChildAttach);

            task.ConfigureAwait(false);

            return task;
        }
    }
}