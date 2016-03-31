using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces;
using Orleans;

namespace WebHost.Controllers
{
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Get([FromUri(Name = "ticket")] int fromId)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IChatRoomGrain>("default");
            var messages = await grain.GetMessagesAsync(fromId);

            HttpResponseMessage response;

            if (messages.Any())
            {
                var dict = new Dictionary<string, string>();

                for (var index = 0; index < messages.Count; index++)
                {
                    var message = messages[index];
                    dict.Add("[" + index + "]", message.Text);
                }

                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new FormUrlEncodedContent(dict);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }

            return response;
        }

        public async Task<HttpResponseMessage> Put([FromBody] Models.PublishMessage message)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IChatUser>(message.User);
            var success = await grain.PublishMessageAsync(new PublishMessage
            {
                Text = message.Text
            });

            HttpResponseMessage response;

            if (success)
            {
                response = Request.CreateResponse(HttpStatusCode.Created);
                response.Headers.Location = new Uri("http://localhost:27444/api/room/");
            }
            else
            {
                response = Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Unable to create");
            }

            return response;
        }
    }
}
