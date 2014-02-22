using Castle.Core;
using Nancy;
using OpenHomeServer.Server.Messaging.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Plugins.Ripper.UI
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
