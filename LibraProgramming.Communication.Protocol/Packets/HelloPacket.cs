using System;
using System.Runtime.Serialization;

namespace LibraProgramming.Communication.Protocol.Packets
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.hello")]
    public class HelloPacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.Hello;
    }

    public enum Command
    {

    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.command")]
    public class CommandPacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.Command;

        [DataMember(Name = "cmd")]
        public Command Command
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.profile")]
    public class ProfileRequestPacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.Profile;

        [DataMember(Name = "id")]
        public Guid UserId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.profile")]
    public class ProfileResponsePacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.Profile;

        [DataMember(Name = "id")]
        public Guid UserId
        {
            get;
            set;
        }

        [DataMember(Name = "name")]
        public string UserName
        {
            get;
            set;
        }
    }
}