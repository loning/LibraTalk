using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using LibraProgramming.Communication.Protocol.Packets;
using LibraProgramming.Hessian;

namespace LibraTalk.Windows.Client.Services
{
    public enum CommunicationServiceState
    {
        Failed,
        NotAssigned,

        Connected,
        EstablishConnection
    }

    public class SocketCommunicationService
    {
        private readonly Uri hostUri;
        private CommunicationServiceState state;
        private MessageWebSocket socket;

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

        public SocketCommunicationService(Uri hostUri)
        {
            this.hostUri = hostUri;
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

                using (var stream = socket.OutputStream.AsStreamForWrite())
                {
                    var packet = new EstablishConnectionPacket();
                    SendPacket(stream, packet);
                }

                State = CommunicationServiceState.Connected;
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);
            }
        }

        public bool WhoAmI()
        {
            if (CommunicationServiceState.Connected != State)
            {
                return false;
            }

            try
            {
                using (var stream = socket.OutputStream.AsStreamForWrite())
                {
                    var packet = new ProfileRequestPacket();
                    SendPacket(stream, packet);
                }

                return true;
            }
            catch (WebException exception)
            {
                State = CommunicationServiceState.Failed;

                var status = WebSocketError.GetStatus(exception.HResult);

                Debug.WriteLine("Error: {0}", status);

                return false;
            }
        }

        private void SendPacket<TPacket>(Stream stream, TPacket packet)
            where TPacket : Packet
        {
            if (null == packet)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            using (var writer = new BinaryWriter(stream))
            {
                byte[] data;

                using (var buffer = new MemoryStream())
                {
                    var serializer = new DataContractHessianSerializer(typeof (TPacket));

                    serializer.WriteObject(buffer, packet);

                    data = buffer.ToArray();
                }

                writer.Write(PacketFrame.Signature);
                writer.Write((ushort) packet.Command);
                writer.Write(data.Length);
                writer.Write(data);
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
            if (SocketMessageType.Binary == args.MessageType)
            {
                var stream = args.GetDataStream().AsStreamForRead();


            }
        }
    }
}