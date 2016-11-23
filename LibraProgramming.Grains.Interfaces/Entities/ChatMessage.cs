using System;

namespace LibraProgramming.Grains.Interfaces.Entities
{
    public class ChatMessage
    {
        public Guid Author
        {
            get;
            set;
        }

        public DateTime Date
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