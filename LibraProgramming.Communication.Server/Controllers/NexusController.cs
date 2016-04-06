using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using LibraProgramming.Communication.Protocol.Packets;
using LibraProgramming.Grains.Interfaces;
using LibraProgramming.Hessian;
using Orleans;

namespace LibraProgramming.Communication.Server.Controllers
{
    public class NexusController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var context = HttpContext.Current;

            if (context.IsWebSocketRequest || context.IsWebSocketRequestUpgrading)
            {
                context.AcceptWebSocketRequest(ProcessWebSocketRequest);
            }

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        private static Task ProcessWebSocketRequest(AspNetWebSocketContext context)
        {
            var ws = context.WebSocket;

            return Task.Run(async () =>
            {
                var token = CancellationToken.None;

                var description = await ReadPacketDescriptionAsync(ws, token);

                if (PacketType.Hello != description.PacketType)
                {
                    throw new Exception();
                }

                var hello = await ReadPacketAsync<HelloPacket>(ws, description, token);

                Debug.WriteLine("[NexusController.ProcessWebSocketRequestAsync]");

                while (true)
                {
                    description = await ReadPacketDescriptionAsync(ws, token);

                    switch (description.PacketType)
                    {
                        case PacketType.Profile:
                        {
                            var request = await ReadPacketAsync<ProfileRequestPacket>(ws, description, token);
                            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(request.UserId);

                            var profile = await user.GetUserProfileAsync();
                            var packet = new ProfileResponsePacket
                            {
                                UserId = request.UserId,
                                UserName = profile.Name
                            };

                            await SendPacketAsync(ws, packet, token);

                            break;
                        }
                    }

                    if (WebSocketState.Open != ws.State)
                    {
                        break;
                    }
                }
            });

            /*while (true)
            {
                if (WebSocketState.Open != ws.State)
                {
                    break;
                }

                var buffer = new ArraySegment<byte>(new byte[1024]);

                await ws.SendAsync(buffer, WebSocketMessageType.Binary, false, CancellationToken.None);
            }*/
        }

        private static async Task<PacketDescription> ReadPacketDescriptionAsync(WebSocket socket, CancellationToken token)
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

                return new PacketDescription((PacketType) packetType, length);
            }
        }

        private static async Task<TPacket> ReadPacketAsync<TPacket>(WebSocket socket, PacketDescription description, CancellationToken token)
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
                var packet = (TPacket) serializer.ReadObject(stream);

                if (description.PacketType != packet.PacketType)
                {
                    throw new Exception();
                }

                return packet;
            }
        }

        private static async Task SendPacketAsync<TPacket>(WebSocket socket, TPacket packet, CancellationToken token)
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
                writer.Write((ushort) packet.PacketType);
                writer.Write(data.Length);
                writer.Write(data);
            }

            var buffer = new ArraySegment<byte>(stream.ToArray());

            await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, token);
        }
    }
}
