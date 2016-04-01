using System;

namespace LibraProgramming.Grains.Interfaces
{
    public class RoomMessage
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

        public string PublisherNick
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