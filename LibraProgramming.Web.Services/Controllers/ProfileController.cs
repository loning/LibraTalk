using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using LibraProgramming.Web.Services.Models;
using Orleans;

namespace LibraProgramming.Web.Services.Controllers
{
    // GET http://localhost:8080/api/profile/d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8
    // PUT http://localhost:8080/api/profile/d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8

    public class ProfileController : ApiController
    {
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (Guid.Empty == id)
            {
                return NotFound();
            }

            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);
            var profile = await user.GetUserProfileAsync();

            return Ok(new UserProfileModel
            {
                Name = profile.Name
            });
        }

        public async Task<IHttpActionResult> Put(Guid id, [FromBody] UserProfileModel profile)
        {
            if (Guid.Empty == id)
            {
                return NotFound();
            }

            if (false == IsModelValid(profile))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            var user = GrainClient.GrainFactory.GetGrain<IChatUser>(id);

            await user.SetProfileAsync(new UserProfile
            {
                Name = profile.Name
            });

            return Ok();
        }

        private static bool IsModelValid(UserProfileModel model)
        {
            if (null == model)
            {
                return false;
            }

            if (String.IsNullOrEmpty(model.Name))
            {
                return false;
            }

            return true;
        }
    }
}
