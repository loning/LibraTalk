using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LibraProgramming.Web.Services.Models
{
    [DataContract]
    public class ExistingRoomModel
    {
        [DataMember(Name = "id")]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember(Name = "alias")]
        public string Alias
        {
            get;
            set;
        }

        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }
    }
}