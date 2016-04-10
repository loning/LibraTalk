using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LibraProgramming.Communication.Server.Core;

namespace LibraProgramming.Communication.Server.Controllers
{
    public class NexusController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var context = HttpContext.Current;

            if (context.IsWebSocketRequest || context.IsWebSocketRequestUpgrading)
            {
                context.AcceptWebSocketRequest(SessionManager.Current.ProcessWebSocketRequest);
            }

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }
    }
}
