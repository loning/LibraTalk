namespace LibraProgramming.Communication.Protocol.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPacket
    {
        PacketType PacketType
        {
            get;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IIncomingPacket : IPacket
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IOutgoingPacket : IPacket
    {
    }
}