using Nancy;
using OpenHomeServer.Server.Web.Providers;

namespace OpenHomeServer.Server.Plugins.Home
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ServerInfoProvider provider)
        {
            Get["/"] = x => View["index.cshtml", new { ServerInfo = provider.GetServerInfo(), Title = "TITLE" }];
        }
    }
}
