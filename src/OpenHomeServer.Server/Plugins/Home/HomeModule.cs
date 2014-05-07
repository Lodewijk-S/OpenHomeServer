using Nancy;

namespace OpenHomeServer.Server.Plugins.Home
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = x => View["index.cshtml", new { Title = "Welcome to your OpenHomeServer" }];
        }
    }
}
