using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using WebHost.Models;

namespace WebHost.Controllers
{
    public class UserController : ApiController
    {
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
            var profile = await grain.GetUserProfileAsync();
            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {
                        "id", profile.Id.ToString("D")
                    },
                    {
                        "name", profile.Name
                    }
                });

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(1.0d)
            };

            return response;
        }

        public async Task<HttpResponseMessage> Put(Guid id, [FromBody] Profile profile)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IChatUser>(id);

            try
            {
                await grain.SetProfileAsync(new UserProfile
                {
                    Id = profile.Id,
                    Name = profile.Name
                });

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }
    }
}