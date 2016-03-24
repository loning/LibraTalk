using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;

namespace LibraTalk.Windows.Client.Services
{
    public class ReceivingMessageEventArgs : EventArgs
    {
        
    }

    public interface IMessageSender
    {
        event TypedEventHandler<IMessageSender, ReceivingMessageEventArgs> MessageReceived;

        Task SendMessageAsync(IDictionary<string, string> message);

        void Receive();
    }
}