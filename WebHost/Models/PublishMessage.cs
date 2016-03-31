using System;

namespace WebHost.Models
{
    public class PublishMessage
    {
        public Guid User
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