using HomeServer8.Server.Providers;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ServerInfoProvider provider)
        {
            Get["/"] = x => View["index.cshtml", new { ServerInfo = provider.GetServerInfo(), Title = "TITLE" }];
        }
    }
}
