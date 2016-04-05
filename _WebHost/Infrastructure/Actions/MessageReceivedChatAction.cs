using LibraProgramming.Grains.Interfaces;

namespace WebHost.Infrastructure.Actions
{
    public class MessageReceivedChatAction : IChatMessageAction
    {
        public RoomMessage Message
        {
            get;
        }

        public MessageReceivedChatAction(RoomMessage message)
        {
            Message = message;
        }
    }
}