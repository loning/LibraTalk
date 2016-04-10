using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Communication.Protocol.Packets;
using LibraProgramming.Hessian;

namespace LibraProgramming.Communication.Server.Core
{
    public static class WebSocketExtensions
    {
        public static async Task<PacketDescription> ReadPacketDescriptionAsync(this WebSocket socket, CancellationToken token)
        {
            var content = new ArraySegment<byte>(new byte[PacketDescription.Length]);
            var result = await socket.ReceiveAsync(content, token);

            if (PacketDescription.Length != result.Count)
            {
                throw new Exception();
            }

            var stream = new MemoryStream(content.Array, content.Offset, content.Count);

            using (var reader = new BinaryReader(stream))
            {
                var signature = reader.ReadUInt16();

                if (PacketDescription.Signature != signature)
                {
                    throw new Exception();
                }

                var packetType = reader.ReadUInt16();
                var length = reader.ReadInt32();

                return new PacketDescription((PacketType)packetType, length);
            }
        }

        public static async Task<TPacket> ReadPacketAsync<TPacket>(this WebSocket socket, PacketDescription description, CancellationToken token)
            where TPacket : IIncomingPacket
        {
            var content = new ArraySegment<byte>(new byte[description.ContentLength]);
            var result = await socket.ReceiveAsync(content, token);

            if (WebSocketMessageType.Binary != result.MessageType)
            {
                throw new Exception();
            }

            if (description.ContentLength != result.Count)
            {
                throw new Exception();
            }

            using (var stream = new MemoryStream(content.Array, content.Offset, content.Count))
            {
                var serializer = new DataContractHessianSerializer(typeof(TPacket));
                var packet = (TPacket)serializer.ReadObject(stream);

                if (description.PacketType != packet.PacketType)
                {
                    throw new Exception();
                }

                return packet;
            }
        }

        public static async Task SendPacketAsync<TPacket>(this WebSocket socket, TPacket packet, CancellationToken token)
            where TPacket : IOutgoingPacket
        {
            if (null == packet)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            var stream = new MemoryStream();

            using (var writer = new BinaryWriter(stream))
            {
                byte[] data;

                using (var content = new MemoryStream())
                {
                    var serializer = new DataContractHessianSerializer(typeof(TPacket));

                    serializer.WriteObject(content, packet);

                    data = content.ToArray();
                }

                writer.Write(PacketDescription.Signature);
                writer.Write((ushort)packet.PacketType);
                writer.Write(data.Length);
                writer.Write(data);
            }

            var buffer = new ArraySegment<byte>(stream.ToArray());

            await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, token);
        }
    }
}