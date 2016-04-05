<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="LibraProgramming.Communication.Server.Bootstraps" %>

<script language="c#" type="text/C#" runat="server">
    protected void Application_Start()
    {
        GlobalConfiguration.Configure(WebApiBootstrap.Register);
    }
</script>