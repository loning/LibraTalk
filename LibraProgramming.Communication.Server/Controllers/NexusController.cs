using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

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
                var input = new ArraySegment<byte>(new byte[1024]);

                while (true)
                {
                    var result = await ws.ReceiveAsync(input, CancellationToken.None);

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
    }
}
