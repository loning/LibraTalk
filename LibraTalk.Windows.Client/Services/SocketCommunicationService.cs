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

        private void SendPacket<TPacket>(Stream stream, TPacket packet)
            where TPacket : PacketFrame
        {
            if (null == packet)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            var frame = CreateFrame(packet);
            var header = CreateHeader(PacketFrame.EstablishCommand, frame.Length);

            stream.Write(header, 0, header.Length);
            stream.Write(frame, 0, frame.Length);
        }

        private static byte[] CreateHeader(ushort command, long length)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(PacketFrame.Signature);
                writer.Write(command);
                writer.Write(length);
            }

            return stream.ToArray();
        }

        private static byte[] CreateFrame(object packet)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractHessianSerializer(packet.GetType());

                serializer.WriteObject(stream, packet);

                return stream.ToArray();
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
        }
    }
}