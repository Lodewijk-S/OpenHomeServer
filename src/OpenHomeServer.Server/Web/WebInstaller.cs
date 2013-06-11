using Nancy;
using OpenHomeServer.Server.Web.Providers;
using StructureMap.Configuration.DSL;

namespace OpenHomeServer.Server.Web
{
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            ForConcreteType<ServerInfoProvider>();            
        }
    }
}
