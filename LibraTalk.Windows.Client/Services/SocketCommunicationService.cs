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

        public async Task ConnectAsync()
        {
            socket = new MessageWebSocket();

            socket.MessageReceived += OnSocketMessageReceived;
            socket.Closed += OnSocketClosed;

            try
            {
                await socket.ConnectAsync(hostUri);

                State = CommunicationServiceState.EstablishConnection;

                await SendPacketAsync(socket.OutputStream.AsStreamForWrite(), new HelloPacket());

                State = CommunicationServiceState.Connected;
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);
            }
        }

        public Task WhoAmI(Guid userid)
        {
            if (CommunicationServiceState.Connected != State)
            {
                return Task.CompletedTask;
            }

            try
            {
                var packet = new ProfileRequestPacket
                {
                    UserId = userid
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

//            var stream = new MemoryStream();

            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                var content = GetSerializedPacket(packet);

                writer.Write(PacketDescription.Signature);
                writer.Write((ushort) packet.PacketType);
                writer.Write(content.Length);
                writer.Write(content);
                writer.Flush();

//                stream.Seek(0L, SeekOrigin.Begin);

                await writer.BaseStream.CopyToAsync(outgoing);
                await outgoing.FlushAsync();
            }
        }

        private static TPacket ReceivePacket<TPacket>(Stream stream)
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
                case PacketType.Profile:
                {
                    var content = packet.Content;
                    ProfileResponsePacket profilePacket;

                    using (var memoryStream = new MemoryStream(content.Array, content.Offset, content.Count))
                    {
                        profilePacket = ReceivePacket<ProfileResponsePacket>(memoryStream);
                    }

                    packetReceived.Invoke(this, new PacketReceivedEventArgs(profilePacket));

                    break;
                }
            }
        }
    }
}