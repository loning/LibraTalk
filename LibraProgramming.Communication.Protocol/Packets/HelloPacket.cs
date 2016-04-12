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

        [DataMember(Name = "sid")]
        public Guid SessionId
        {
            get;
            set;
        }
    }

    [DataContract(Name = "packet.message")]
    public class PublishMessagePacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.PublishMessageRequest;

        [DataMember(Name = "text")]
        public string Message
        {
            get;
            set;
        }
    }

    public enum Command
    {
        QueryProfile = 0x01,
        QueryTime
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.command")]
    public class CommandPacket : IOutgoingPacket, IIncomingPacket
    {
        public virtual PacketType PacketType => PacketType.Command;

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
    [DataContract(Name = "packet.command.payload")]
    public class CommandPacket<TPayload> : CommandPacket
    {
        public override PacketType PacketType => PacketType.CommandWithPayload;

        [DataMember(Name = "payload")]
        public TPayload Payload
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.profile")]
    public class QueryProfileResponsePacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.QueryProfileResponse;

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

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.profile")]
    public class UpdateProfileRequestPacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.UpdateProfileRequest;

        [DataMember(Name = "name")]
        public string UserName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "packet.datetime")]
    public class DateTimeResponsePacket : IOutgoingPacket, IIncomingPacket
    {
        public PacketType PacketType => PacketType.DateTime;

        [DataMember(Name = "now")]
        public DateTime NowUtc
        {
            get;
            set;
        }
    }
}