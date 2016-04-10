using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;
using LibraProgramming.Communication.Protocol.Packets;

namespace LibraProgramming.Communication.Server.Core
{
    public class SessionManager
    {
        public static SessionManager Current
        {
            get;
        }

        public ConcurrentDictionary<Guid, SessionClient> Clients;
         
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        private SessionManager()
        {
            Clients = new ConcurrentDictionary<Guid, SessionClient>();
        }

        static SessionManager()
        {
            Current = new SessionManager();
        }

        public async Task ProcessWebSocketRequest(AspNetWebSocketContext context)
        {
            var websocket = context.WebSocket;
            var token = CancellationToken.None;
            var description = await websocket.ReadPacketDescriptionAsync(token);

            if (PacketType.Hello != description.PacketType)
            {
                throw new Exception();
            }

            var hello = await websocket.ReadPacketAsync<HelloPacket>(description, token);
            var client = new SessionClient(websocket, hello.SessionId);

//            if (Clients.TryAdd(hello.SessionId, client))
//            {
                await client.StartAsync();
//            }
        }
    }
}