using System;
using System.ComponentModel.DataAnnotations;
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
using LibraProgramming.Communication.Server.Core;
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
                context.AcceptWebSocketRequest(ProcessWebSocketRequestAsync);
            }

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        private static async Task ProcessWebSocketRequestAsync(AspNetWebSocketContext context)
        {
            var ws = context.WebSocket;

            new Task(async () =>
            {
                var handshake = await ReadPacketAsync<EstablishConnectionPacket>(ws);

                Debug.WriteLine("[NexusController.ProcessWebSocketRequestAsync]");

                while (true)
                {
                    var header = await ReadPacketHeaderAsync(ws);

                    switch (header.Command)
                    {
                        case LibraTalkCommand.RequestProfile:
                        {
                            var request = await ReadPacketFrameAsync<ProfileRequestPacket>(ws, header.FrameLength);
                            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(request.UserId);
                            var profile = await user.GetUserProfileAsync();

                            await SendPacketAsync(ws, new ProfileResponsePacket
                            {
                                UserName = profile.Name
                            });

                            break;
                        }
                    }

                    /*var result = await ws.ReceiveAsync(header, CancellationToken.None);

                    if (WebSocketMessageType.Binary == result.MessageType && !result.EndOfMessage)
                    {
                        var stream = new MemoryStream(header.Array, header.Offset, header.Count);

                        using (var reader = new BinaryReader(stream))
                        {
                            var signature = reader.ReadUInt16();

                            if (PacketFrame.Signature != signature)
                            {
                                throw new Exception();
                            }

                            var command = reader.ReadUInt16();
                            var length = reader.ReadUInt64();

                            switch (command)
                            {
                                case PacketFrame.EstablishCommand:
                                {
                                    var packet = new ArraySegment<byte>(new byte[length]);

                                    await ws.ReceiveAsync(packet, CancellationToken.None);


                                    break;
                                }
                            }
                        }
                    }*/
                    if (WebSocketState.Open != ws.State)
                    {
                        break;
                    }
                }
            }).Start();

            while (true)
            {
                if (WebSocketState.Open != ws.State)
                {
                    break;
                }

                var buffer = new ArraySegment<byte>(new byte[1024]);

                await ws.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }

        private static async Task<PacketFrameHeader> ReadPacketHeaderAsync(WebSocket socket)
        {
            const int headerLength = 8;
            var header = new ArraySegment<byte>(new byte[headerLength]);
            var result = await socket.ReceiveAsync(header, CancellationToken.None);

            if (headerLength != result.Count)
            {
                throw new Exception();
            }

            var stream = new MemoryStream(header.Array, header.Offset, header.Count);

            using (var reader = new BinaryReader(stream))
            {
                var signature = reader.ReadUInt16();

                if (PacketFrame.Signature != signature)
                {
                    throw new Exception();
                }

                var command = reader.ReadUInt16();
                var length = reader.ReadInt32();

                return new PacketFrameHeader((LibraTalkCommand) command, length);
            }
        }

        private static async Task<TPacket> ReadPacketFrameAsync<TPacket>(WebSocket socket, int contentLength)
            where TPacket : Packet
        {
            var frame = new ArraySegment<byte>(new byte[contentLength]);
            var result = await socket.ReceiveAsync(frame, CancellationToken.None);

            if (contentLength != result.Count)
            {
                throw new Exception();
            }

            using (var stream = new MemoryStream(frame.Array, frame.Offset, frame.Count))
            {
                var serializer = new DataContractHessianSerializer(typeof (TPacket));
                return (TPacket) serializer.ReadObject(stream);
            }
        }

        private static async Task<TPacket> ReadPacketAsync<TPacket>(WebSocket socket)
            where TPacket : Packet
        {
            var header = await ReadPacketHeaderAsync(socket);
            return await ReadPacketFrameAsync<TPacket>(socket, header.FrameLength);
        }

        private static async Task SendPacketAsync<TPacket>(WebSocket socket, TPacket packet)
            where TPacket : Packet
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

                writer.Write(PacketFrame.Signature);
                writer.Write((ushort) packet.Command);
                writer.Write(data.Length);
                writer.Write(data);
            }

            var buffer = new ArraySegment<byte>(stream.ToArray());

            await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
