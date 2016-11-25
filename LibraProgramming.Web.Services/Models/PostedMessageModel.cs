using System.Runtime.Serialization;

namespace LibraProgramming.Web.Services.Models
{
    [DataContract]
    public class PostedMessageModel
    {
        [DataMember(Name = "author")]
        public UserProfileModel Author
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