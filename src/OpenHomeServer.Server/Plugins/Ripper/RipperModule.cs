using Nancy;

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
            return new RipperViewModel
            {
                Title="Ripper",
                Status = service.GetCurrentStatus()
            };
        }
    }

    public class RipperViewModel
    {
        public string Title { get; set; }
        public RippingStatus Status { get; set; }
    }
}
