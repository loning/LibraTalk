namespace LibraProgramming.Communication.Protocol.Packets
{
    public class PacketFrame
    {
        public const ushort Signature = 0xABCD;
        public const ushort EstablishCommand = 0x0001;

        public PacketFrameHeader Header
        {
            get;
        }

        public Packet Packet
        {
            get;
        }

        public PacketFrame(PacketFrameHeader header, Packet packet)
        {
            Header = header;
            Packet = packet;
        }
    }
}