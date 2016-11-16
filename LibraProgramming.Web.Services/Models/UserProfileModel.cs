using System;
using System.Runtime.Serialization;

namespace LibraProgramming.Web.Services.Models
{
    [Serializable]
    [DataContract]
    public sealed class UserProfileModel
    {
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}