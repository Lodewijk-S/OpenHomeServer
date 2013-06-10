using OpenHomeServer.Server.Web.Providers;
using Nancy;
using System;

namespace OpenHomeServer.Server.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ServerInfoProvider provider)
        {
            Get["/"] = x =>{
                return View["index.cshtml", new { ServerInfo = provider.GetServerInfo(), Title = "TITLE" }];
            };
        }
    }
}
