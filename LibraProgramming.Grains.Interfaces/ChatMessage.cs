using System;

namespace LibraProgramming.Grains.Interfaces
{
    [Serializable]
    public class ChatMessage
    {
         public long Id
         {
             get;
             set;
         }

        public Guid PublisherId
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }
    }
}