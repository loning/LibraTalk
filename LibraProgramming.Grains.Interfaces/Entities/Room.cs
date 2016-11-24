using System;

namespace LibraProgramming.Grains.Interfaces.Entities
{
    public sealed class Room
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}