using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using WebHost.Models;

namespace WebHost.Controllers
{
    public class RoomController : ApiController
    {
        public async Task<HttpResponseMessage> Put(string id, [FromBody] User user)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IChatRoomGrain>(id);

            await grain.AddUserAsync(user.Id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
