using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LibraProgramming.WebApiProxy.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();
            app.UseWebSockets();
            app.Use(HandleWebSocket);
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        private async Task HandleWebSocket(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var websocket = await context.WebSockets.AcceptWebSocketAsync();

                if (null != websocket)
                {
                    while (WebSocketState.Open == websocket.State)
                    {
                        var token = CancellationToken.None;
                        var buffer = new ArraySegment<byte>(new byte[1024]);

                        var receive = await websocket.ReceiveAsync(buffer, token);


                    }
                }
            }
            else
            {
                await next();
            }
        }
    }
}
