using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;

namespace LibraTalk.Windows.Client.Services
{
    public class ReceivingMessageEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.CancelEventArgs"/> class with the <see cref="P:System.ComponentModel.CancelEventArgs.Cancel"/> property set to the given value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false. </param>
        public ReceivingMessageEventArgs(bool cancel)
            : base(cancel)
        {
        }
    }

    public interface IMessageSender
    {
//        event TypedEventHandler<IMessageSender, ReceivingMessageEventArgs> MessageReceived;

        Task<string> GetUserNameAsync(Guid id);

        Task SetUserName(Guid id, string name);

//        Task SendMessageAsync(IDictionary<string, string> message);

        //        void Receive();
    }
}