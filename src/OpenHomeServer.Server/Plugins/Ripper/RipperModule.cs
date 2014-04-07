using Nancy;
using Newtonsoft.Json;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperModule : NancyModule
    {
        public RipperModule(RipperService service)
            : base("ripper")
        {
            Get["/"] = x => View["index.cshtml", GetRipperViewModel(service)];
        }

        private RipperViewModel GetRipperViewModel(RipperService service)
        {
            var status = service.GetCurrentStatus();

            return new RipperViewModel
            {
                Title="Ripper",
                View = status == null ? "null" : JsonConvert.SerializeObject(status)
            };
        }
    }

    public class RipperViewModel
    {
        public string Title { get; set; }
        public string View { get; set; }
    }
}
