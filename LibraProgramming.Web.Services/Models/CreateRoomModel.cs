using System.Runtime.Serialization;

namespace LibraProgramming.Web.Services.Models
{
    [DataContract]
    public class CreateRoomModel
    {
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }
    }
}