using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces.Grains;
using LibraProgramming.Web.Services.Models;
using Orleans;

namespace LibraProgramming.Web.Services.Controllers
{
    // PUT http://localhost:8080/api/chat/room1
    // DELETE http://localhost:8080/api/chat/room1

    public class ChatController : ApiController
    {
        public async Task<IHttpActionResult> Put(string id, [FromBody] UserDescriptorModel descriptor)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (false == IsModelValid(descriptor))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChatRoom>(id);
            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(descriptor.User);

            await chat.JoinAsync(user);

            return Ok();
        }

        public async Task<IHttpActionResult> Delete(string id, [FromBody] UserDescriptorModel descriptor)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (false == IsModelValid(descriptor))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChatRoom>(id);
            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(descriptor.User);

            await chat.LeaveAsync(user);

            return Ok();
        }

        private static bool IsModelValid(UserDescriptorModel model)
        {
            if (null == model)
            {
                return false;
            }

            if (Guid.Empty == model.User)
            {
                return false;
            }

            return true;
        }
    }
}
