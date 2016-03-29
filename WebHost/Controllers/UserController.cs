using System;
using System.Threading.Tasks;
using System.Web.Http;
using WebHost.Models;

namespace WebHost.Controllers
{
    public class UserController : ApiController
    {
        public async Task<IHttpActionResult> Get(Guid id)
        {
//            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
//            return user.GetName();
            await Task.Delay(TimeSpan.FromSeconds(1.0d));

            return Ok(new
            {
                Profile = new ChatUserProfile
                {
                    Id = id,
                    Name = "John Doe"
                }
            });
        }

        public async Task<IHttpActionResult> Post(Guid id, [FromBody] ChatUser user)
        {
//            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);

//            await user.SetName(userName);
            await Task.Delay(TimeSpan.FromSeconds(1.0d));

            return Ok();
        }
    }
}