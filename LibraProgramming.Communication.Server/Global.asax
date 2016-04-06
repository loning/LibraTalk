<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Hosting" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="LibraProgramming.Communication.Server.Bootstraps" %>
<%@ Import Namespace="Orleans" %>

<script language="c#" type="text/C#" runat="server">

    protected void Application_Start()
    {
        var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientConfiguration.xml");

        GlobalConfiguration.Configure(WebApiBootstrap.Register);
        GrainClient.Initialize(path);
    }

</script>