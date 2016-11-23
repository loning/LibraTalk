using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using LibraProgramming.Web.Services.Core;
using Orleans;
using Orleans.Streams;

namespace LibraProgramming.Web.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PollController : ApiController
    {
        private readonly IStreamProvider provider;

        public PollController()
        {
            provider = GrainClient.GetStreamProvider("SMSProvider");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri(Name = "id")] string alias)
        {
            const string test = "$ROOMS";

            var rooms = GrainClient.GrainFactory.GetGrain<IRoomsProvider>(test);
            var id = await rooms.GetOrAddRoomAsync(alias);
            var stream = provider.GetStream<ChatMessage>(id, test);

            var observer = new ChatObserver();

            try
            {
                await observer.SubscribeAsync(stream);
                var messages = await observer.WaitForUpdates(TimeSpan.FromMinutes(1.0d), CancellationToken.None);
            }
            finally
            {
                await observer.ReleaseAsync();
            }

            return Ok();
        }
    }
}
