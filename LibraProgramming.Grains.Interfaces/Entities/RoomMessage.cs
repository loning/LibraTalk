using System;

namespace LibraProgramming.Grains.Interfaces.Entities
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