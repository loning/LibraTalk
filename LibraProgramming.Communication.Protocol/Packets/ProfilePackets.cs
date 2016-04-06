using System;
using System.Runtime.Serialization;

namespace LibraProgramming.Communication.Protocol.Packets
{
    [DataContract(Name = "profile.request")]
    public class ProfileRequestPacket : Packet
    {
        public override LibraTalkCommand Command => LibraTalkCommand.RequestProfile;

        [DataMember(Name = "id")]
        public Guid UserId
        {
            get;
            set;
        }
    }

    [DataContract(Name = "profile.response")]
    public class ProfileResponsePacket : Packet
    {
        public override LibraTalkCommand Command => LibraTalkCommand.ProfileResponse;

        [DataMember(Name = "name")]
        public string UserName
        {
            get;
            set;
        }
    }
}