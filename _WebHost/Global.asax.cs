using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Orleans;

namespace WebHost
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientConfiguration.xml");

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GrainClient.Initialize(path);
            MonitorsBootstrap.Register();
        }
    }
}
