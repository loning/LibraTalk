namespace LibraProgramming.Communication.Protocol.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public enum PacketType : ushort
    {
        /// <summary>
        /// 
        /// </summary>
        Hello,

        /// <summary>
        /// 
        /// </summary>
        Profile,

        /// <summary>
        /// 
        /// </summary>
        Command
    }

    public class PacketDescription
    {
        public const int Length = 0x08;
        public const ushort Signature = 0xABCD;

        public PacketType PacketType
        {
            get;
            set;
        }

        public int ContentLength
        {
            get;
        }

        public PacketDescription(PacketType packetType, int contentLength)
        {
            PacketType = packetType;
            ContentLength = contentLength;
        }
    }
}