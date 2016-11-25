using System;
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
    public class MessageController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="model"></param>
        /// <returns>
        /// </returns>
        public async Task<IHttpActionResult> Post([FromUri(Name = "id")] string alias, [FromBody] PostMessageModel model)
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

            await chat.PublishMessageAsync(alias, model.Author, new UserMessage
            {
                Text = model.Text
            });

            var link = Url.Link("DefaultApi", new
            {
                id = Guid.NewGuid()
            });

            return Created(new Uri(link), new PostedMessageModel());
        }

        private static bool IsModelValid(PostMessageModel model)
        {
            return null != model && false == String.IsNullOrEmpty(model.Text);
        }
    }
}
