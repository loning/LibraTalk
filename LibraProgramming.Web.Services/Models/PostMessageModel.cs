using System;
using System.Runtime.Serialization;

namespace LibraProgramming.Web.Services.Models
{
    [DataContract]
    public class PostMessageModel
    {
        [DataMember(Name = "author")]
        public Guid Author
        {
            get;
            set;
        }

        [DataMember(Name = "text")]
        public string Text
        {
            get;
            set;
        }
    }
}