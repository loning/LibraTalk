using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces.Grains;
using LibraProgramming.Web.Services.Models;
using Orleans;

namespace LibraProgramming.Web.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatController : ApiController
    {
        /// <summary>
        /// GET http://localhost:8080/api/chat/room1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChatRoom>(id);
            var users = await chat.GetUsersAsync();

            return Ok(
                users
                    .Select(user => new UserDescriptorModel
                    {
                        User = user.GetPrimaryKey()
                    })
                    .ToArray()
            );
        }

        /// <summary>
        /// PUT http://localhost:8080/api/chat/room1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
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

            var rooms = GrainClient.GrainFactory.GetGrain<IActiveChatRooms>(0);
            var room = await rooms.GetRoomAsync(id);


            var chat = GrainClient.GrainFactory.GetGrain<IChatRoom>(id);
            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(descriptor.User);

            await chat.JoinAsync(user);

            return Ok();
        }

        /// <summary>
        /// DELETE http://localhost:8080/api/chat/room1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
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
