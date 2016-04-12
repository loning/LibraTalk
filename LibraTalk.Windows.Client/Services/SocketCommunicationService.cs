using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using LibraProgramming.Communication.Protocol.Packets;
using LibraProgramming.Hessian;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraTalk.Windows.Client.Services
{
    public enum CommunicationServiceState
    {
        Failed,
        NotAssigned,

        Connected,
        EstablishConnection
    }

    public class PacketReceivedEventArgs : EventArgs
    {
        public IIncomingPacket Packet
        {
            get;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.EventArgs"/>.
        /// </summary>
        public PacketReceivedEventArgs(IIncomingPacket packet)
        {
            Packet = packet;
        }
    }

    public class SocketCommunicationService
    {
        private readonly Uri hostUri;
        private CommunicationServiceState state;
        private MessageWebSocket socket;
        private readonly WeakEvent<TypedEventHandler<SocketCommunicationService, PacketReceivedEventArgs>> packetReceived;

        public CommunicationServiceState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == state)
                {
                    return;
                }

                state = value;

                DoStateChanged();
            }
        }

        public event TypedEventHandler<SocketCommunicationService, PacketReceivedEventArgs> PacketReceived
        {
            add
            {
                packetReceived.AddHandler(value);
            }
            remove
            {
                packetReceived.RemoveHandler(value);
            }
        }

        public SocketCommunicationService(Uri hostUri)
        {
            this.hostUri = hostUri;
            packetReceived = new WeakEvent<TypedEventHandler<SocketCommunicationService, PacketReceivedEventArgs>>();
        }

        public async Task ConnectAsync(Guid sessionId)
        {
            socket = new MessageWebSocket();

            socket.MessageReceived += OnSocketMessageReceived;
            socket.Closed += OnSocketClosed;

            try
            {
                await socket.ConnectAsync(hostUri);

                State = CommunicationServiceState.EstablishConnection;

                await SendPacketAsync(socket.OutputStream.AsStreamForWrite(), new HelloPacket
                {
                    SessionId = sessionId
                });

                State = CommunicationServiceState.Connected;
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);
            }
        }

        public Task GetNameAsync()
        {
            if (CommunicationServiceState.Connected != State)
            {
                return Task.CompletedTask;
            }

            try
            {
                var packet = new CommandPacket
                {
                    Command = Command.QueryProfile
                };

                return SendPacketAsync(socket.OutputStream.AsStreamForWrite(), packet);
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);

                return Task.CompletedTask;
            }
        }

        public Task SetNameAsync(string name)
        {
            if (CommunicationServiceState.Connected != State)
            {
                return Task.CompletedTask;
            }

            try
            {
                var packet = new UpdateProfileRequestPacket
                {
                    UserName = name
                };

                return SendPacketAsync(socket.OutputStream.AsStreamForWrite(), packet);
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);

                return Task.CompletedTask;
            }
        }

        public Task QueryTimeAsync()
        {
            if (CommunicationServiceState.Connected != State)
            {
                return Task.CompletedTask;
            }

            try
            {
                var packet = new CommandPacket
                {
                    Command = Command.QueryTime
                };

                return SendPacketAsync(socket.OutputStream.AsStreamForWrite(), packet);
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);

                return Task.CompletedTask;
            }
        }

        public Task SendMessageAsync(string text)
        {
            if (CommunicationServiceState.Connected != State)
            {
                return Task.CompletedTask;
            }

            try
            {
                var packet = new PublishMessagePacket
                {
                    Message = text
                };

                return SendPacketAsync(socket.OutputStream.AsStreamForWrite(), packet);
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);

                return Task.CompletedTask;
            }
        }

        private static byte[] GetSerializedPacket<TPacket>(TPacket packet)
            where TPacket : IOutgoingPacket
        {
            using (var buffer = new MemoryStream())
            {
                var serializer = new DataContractHessianSerializer(typeof (TPacket));

                serializer.WriteObject(buffer, packet);

                return buffer.ToArray();
            }
        }

        private async Task SendPacketAsync<TPacket>(Stream outgoing, TPacket packet)
            where TPacket : IOutgoingPacket
        {
            if (null == packet)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                var content = GetSerializedPacket(packet);

                writer.Write(PacketDescription.Signature);
                writer.Write((ushort) packet.PacketType);
                writer.Write(content.Length);
                writer.Write(content);
                writer.Flush();

                writer.BaseStream.Seek(0L, SeekOrigin.Begin);

                await writer.BaseStream.CopyToAsync(outgoing);
                await outgoing.FlushAsync();
            }
        }

        private static TPacket LoadPacketContent<TPacket>(Stream stream)
            where TPacket : IIncomingPacket
        {
            var serializer = new DataContractHessianSerializer(typeof (TPacket));
            return (TPacket) serializer.ReadObject(stream);
        }

        private static Packet ReadPacket(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                var signature = reader.ReadUInt16();

                if (PacketDescription.Signature != signature)
                {
                    throw new Exception();
                }

                var packetType = reader.ReadUInt16();
                var length = reader.ReadInt32();
                var bytes = reader.ReadBytes(length);

                return new Packet((PacketType) packetType, length, bytes);
            }
        }

        private void DoStateChanged()
        {
        }

        private void OnSocketClosed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
        }

        private void OnSocketMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            if (SocketMessageType.Binary != args.MessageType)
            {
                return;
            }

            var packet = ReadPacket(args.GetDataStream().AsStreamForRead());

            switch (packet.PacketType)
            {
                case PacketType.QueryProfileResponse:
                {
                    var content = packet.Content;
                    QueryProfileResponsePacket response;

                    using (var memoryStream = new MemoryStream(content.Array, content.Offset, content.Count))
                    {
                        response = LoadPacketContent<QueryProfileResponsePacket>(memoryStream);
                    }

                    packetReceived.Invoke(this, new PacketReceivedEventArgs(response));

                    break;
                }

                case PacketType.DateTime:
                {
                    var content = packet.Content;
                    DateTimeResponsePacket response;

                    using (var memoryStream = new MemoryStream(content.Array, content.Offset, content.Count))
                    {
                        response = LoadPacketContent<DateTimeResponsePacket>(memoryStream);
                    }

                    packetReceived.Invoke(this, new PacketReceivedEventArgs(response));

                    break;
                }
            }
        }
    }
}