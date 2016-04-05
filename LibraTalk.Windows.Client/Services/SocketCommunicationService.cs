using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace LibraTalk.Windows.Client.Services
{
    public class SocketCommunicationService
    {
        private readonly Uri hostUri;
        private MessageWebSocket socket;

        public SocketCommunicationService(Uri hostUri)
        {
            this.hostUri = hostUri;
            socket = new MessageWebSocket();
            socket.MessageReceived += OnSocketMessageReceived;
            socket.Closed += OnSocketClosed;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await socket.ConnectAsync(hostUri);
            }
            catch (WebException exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void OnSocketClosed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnSocketMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}