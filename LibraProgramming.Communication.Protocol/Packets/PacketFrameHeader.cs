namespace LibraProgramming.Communication.Protocol.Packets
{
    public class PacketFrameHeader
    {
        public LibraTalkCommand Command
        {
            get;
        }

        public int FrameLength
        {
            get;
        }

        public PacketFrameHeader(LibraTalkCommand command, int frameLength)
        {
            Command = command;
            FrameLength = frameLength;
        }
    }
}