using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebHost.Models;

namespace WebHost.Controllers
{
    public class UserController : ApiController
    {
        public async Task<HttpResponseMessage> Get(Guid id)
        {
//            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
//            return user.GetName();

            var response = Request.CreateResponse(HttpStatusCode.OK);

            await Task.Delay(TimeSpan.FromSeconds(1.0d));

            response.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {"id", Guid.NewGuid().ToString("D")},
                    {"name", "John Dow"}
                });

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(1.0d)
            };

            return response;
        }

        public async Task<HttpResponseMessage> Put(Guid id, [FromBody] Profile user)
        {
//            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);

//            await user.SetName(userName);
            var response = Request.CreateResponse(HttpStatusCode.OK);

            await Task.Delay(TimeSpan.FromSeconds(1.0d));

            return response;
        }
    }
}