using HomeServer8.Server.Web.Providers;
using Nancy;
using System;

namespace HomeServer8.Server.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ServerInfoProvider provider)
        {
            Get["/"] = x => View["index.cshtml", new { ServerInfo = provider.GetServerInfo(), Title = "TITLE" }];
        }
    }
}
