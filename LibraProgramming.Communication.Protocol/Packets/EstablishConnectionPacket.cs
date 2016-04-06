using System.Runtime.Serialization;

namespace LibraProgramming.Communication.Protocol.Packets
{
    [DataContract(Name = "establish")]
    public class EstablishConnectionPacket : Packet
    {
        public override LibraTalkCommand Command => LibraTalkCommand.EstablishConnection;
    }
}