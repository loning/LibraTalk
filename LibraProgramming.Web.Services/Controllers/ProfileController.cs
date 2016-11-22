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
    /// <summary>
    /// 
    /// </summary>
    public class ProfileController : ApiController
    {
        /// <summary>
        /// GET http://localhost:8080/api/profile/d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (Guid.Empty == id)
            {
                return NotFound();
            }

            var user = GrainClient.GrainFactory.GetGrain<IUserProfile>(id);
            var profile = await user.GetProfileAsync();

            return Ok(new UserProfileModel
            {
                Name = profile.Name
            });
        }

        /// <summary>
        /// PUT http://localhost:8080/api/profile/d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
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

            var user = GrainClient.GrainFactory.GetGrain<IUserProfile>(id);

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
