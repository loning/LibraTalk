using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LibraProgramming.Grains.Interfaces.Grains;
using LibraProgramming.Web.Services.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Orleans;

namespace LibraProgramming.Web.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomController : ApiController
    {
        /// <summary>
        /// GET http://localhost:8080/api/room/test1
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri(Name = "id")] string alias)
        {
            if (null == alias)
            {
                return NotFound();
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var room = await chat.GetRoomAsync(alias);

            return Ok(new ExistingRoomModel
            {
                Id = room.Id,
                Alias = room.Alias,
                Description = room.Description
            });
        }

        /// <summary>
        /// PUT http://localhost:8080/api/room/test1
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post([FromUri(Name = "id")] string alias, [FromBody] CreateRoomModel model)
        {
            if (null == alias)
            {
                return NotFound();
            }

            if (false == IsModelValid(model))
            {
                return BadRequest("Bad parameters");
            }

            var chat = GrainClient.GrainFactory.GetGrain<IChat>(0);
            var room = await chat.RegisterRoomAsync(alias, model.Description);
            var link = Url.Link("DefaultApi", new { id = room.Id });

            return Created(new Uri(link), new ExistingRoomModel
            {
                Id = room.Id,
                Alias = room.Alias,
                Description = room.Description
            });
        }

        private static bool IsModelValid(CreateRoomModel model)
        {
            return null != model && false == String.IsNullOrEmpty(model.Description);
        }
    }
}
