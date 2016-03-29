using System;

namespace WebHost.Models
{
    [Serializable]
    public class ChatUser
    {
        public ChatUserProfile Profile
        {
            get;
            set;
        } 
    }
}