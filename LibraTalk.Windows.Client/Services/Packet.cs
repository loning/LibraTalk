using System;
using LibraProgramming.Communication.Protocol.Packets;

namespace LibraTalk.Windows.Client.Services
{
    public sealed class Packet : PacketDescription
    {
        public ArraySegment<byte> Content
        {
            get;
        }

        public Packet(PacketType packetType, int contentLength, byte[] content)
            : base(packetType, contentLength)
        {
            Content = new ArraySegment<byte>(content);
        }
    }
}