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
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri(Name = "id")] string alias)
        {
            if (String.IsNullOrEmpty(alias))
            {
                return NotFound();
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var users = await chat.GetUsersAsync(alias);

            return Ok(
                users
                    .Select(user => new ChatUserModel
                    {
                        User = user.GetPrimaryKey()
                    })
                    .ToArray()
            );
        }

        /// <summary>
        /// PUT http://localhost:8080/api/chat/room1
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Put([FromUri(Name = "id")] string alias, [FromBody] ChatUserModel model)
        {
            if (String.IsNullOrEmpty(alias))
            {
                return NotFound();
            }

            if (false == IsModelValid(model))
            {
                return BadRequest();
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var succeeded = await chat.JoinUserAsync(alias, model.User);

            if (succeeded)
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// DELETE http://localhost:8080/api/chat/room1
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Delete([FromUri(Name = "id")] string alias, [FromBody] ChatUserModel model)
        {
            if (String.IsNullOrEmpty(alias))
            {
                return NotFound();
            }

            if (false == IsModelValid(model))
            {
                return BadRequest();
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var succeeded = await chat.LeaveUserAsync(alias, model.User);

            if (succeeded)
            {
                return Ok();
            }

            return NotFound();
        }

        private static bool IsModelValid(ChatUserModel model)
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
