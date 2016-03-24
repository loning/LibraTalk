using System;
using WebHost.Infrastructure.Actions;

namespace WebHost.Infrastructure
{
    public class ChatPollTask : IObserver<IChatMessageAction>
    {
        private IDisposable token;

        public void SubscribeTo(IObservable<IChatMessageAction> provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            token = provider.Subscribe(this);
        }

        void IObserver<IChatMessageAction>.OnNext(IChatMessageAction value)
        {
            throw new NotImplementedException();
        }

        void IObserver<IChatMessageAction>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<IChatMessageAction>.OnCompleted()
        {
            if (null == token)
            {
                return;
            }

            token.Dispose();

            token = null;
        }
    }
}