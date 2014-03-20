using Nancy;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperModule : NancyModule
    {
        public RipperModule()
            : base("ripper")
        {
            Get["/"] = x => View["index.cshtml"];
        }

    }    
}
