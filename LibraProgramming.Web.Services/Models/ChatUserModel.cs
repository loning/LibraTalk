using System;
using System.Runtime.Serialization;

namespace LibraProgramming.Web.Services.Models
{
    [Serializable]
    [DataContract]
    public class ChatUserModel
    {
        [DataMember(Name = "user")]
        public Guid User
        {
            get;
            set;
        }
    }
}