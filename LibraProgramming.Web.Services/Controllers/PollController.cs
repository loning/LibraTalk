using System;
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
    public class PollController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri(Name = "id")] string alias)
        {
            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var room = await chat.GetRoomAsync(alias);

            if (null == room)
            {
                return NotFound();
            }

            var observer = GrainClient.GrainFactory.GetGrain<IChatObserver>(room.Id);
            var message = await observer.WaitForUpdates(TimeSpan.FromMinutes(1.0d));

            if (null == message)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            var user = GrainClient.GrainFactory.GetGrain<IUserProfile>(message.Author);
            var profile = await user.GetProfileAsync();

            return Ok(new PostedMessageModel
            {
                Author = new UserProfileModel
                {
                    Name = profile.Name
                },
                Text = message.Text
            });
        }
    }
}
