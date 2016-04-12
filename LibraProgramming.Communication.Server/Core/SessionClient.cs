using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Communication.Protocol.Packets;
using LibraProgramming.Grains.Interfaces;
using Orleans;

namespace LibraProgramming.Communication.Server.Core
{
    public class SessionClient
    {
        private readonly CancellationToken cancellationToken;

        public Guid SessionId
        {
            get;
        }

        public WebSocket Socket
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SessionClient(WebSocket socket, Guid sessionId)
        {
            Socket = socket;
            SessionId = sessionId;
            cancellationToken = CancellationToken.None;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                var description = await Socket.ReadPacketDescriptionAsync(cancellationToken);

                switch (description.PacketType)
                {
                    case PacketType.Command:
                    {
                        var packet = await Socket.ReadPacketAsync<CommandPacket>(description, cancellationToken);

                        switch (packet.Command)
                        {
                            case Command.QueryProfile:
                            {
                                var user = GrainClient.GrainFactory.GetGrain<IChatUser>(SessionId);
                                var profile = await user.GetUserProfileAsync();

                                await Socket.SendPacketAsync(new QueryProfileResponsePacket
                                {
                                    UserId = SessionId,
                                    UserName = profile.Name
                                }, cancellationToken);

                                break;
                            }

                            case Command.QueryTime:
                            {
                                var chat = GrainClient.GrainFactory.GetGrain<IDateTimeGrain>("server");
                                var datetime = await chat.GetTimeAsync();

                                await Socket.SendPacketAsync(new DateTimeResponsePacket
                                {
                                    NowUtc = datetime
                                }, cancellationToken);

                                break;
                            }
                        }

                        break;
                    }

                    case PacketType.UpdateProfileRequest:
                    {
                        var request = await Socket.ReadPacketAsync<UpdateProfileRequestPacket>(description, cancellationToken);
                        var user = GrainClient.GrainFactory.GetGrain<IChatUser>(SessionId);

                        var profile = new UserProfile
                        {
                            Name = request.UserName
                        };

                        var packet = new QueryProfileResponsePacket
                        {
                            UserId = SessionId,
                            UserName = request.UserName
                        };

                        await Task
                            .WhenAll(
                                user.SetProfileAsync(profile),
                                Socket.SendPacketAsync(packet, cancellationToken)
                            );

                        break;
                    }

                    case PacketType.PublishMessageRequest:
                    {
                        var request = await Socket.ReadPacketAsync<PublishMessagePacket>(description, cancellationToken);

                        break;
                    }
                }

                if (WebSocketState.Open != Socket.State)
                {
                    break;
                }
            }
        }
    }
}