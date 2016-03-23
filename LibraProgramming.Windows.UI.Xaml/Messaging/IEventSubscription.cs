using System.Threading.Tasks;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    public interface IEventSubscription
    {
        SubscriptionToken Token
        {
            get;
            set;
        }

        Task ExecuteAsync(params object[] args);
    }
}