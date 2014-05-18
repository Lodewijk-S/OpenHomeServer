﻿using Nancy;
using Nancy.Extensions;
using Nancy.Responses;
using Newtonsoft.Json;
using OpenHomeServer.Server.Plugins.Ripper.Domain;
using OpenHomeServer.Server.Storage;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperModule : NancyModule
    {
        public RipperModule(RipperService service)
            : base("ripper")
        {
            Get["/"] = x => View["index.cshtml", GetRipperViewModel(service)];
            Post["/Cancel"] = x => 
            {
                service.CancelRipping();
                if (Context.IsAjaxRequest())
                {
                    return "ok";
                } 
                return new RedirectResponse(ModulePath);
            };
            Post["/selectAlbum"] = x =>
            {
                service.SelectAlbum(Request.Form.albumId);

                if (Context.IsAjaxRequest())
                {
                    return "ok";
                }
                return new RedirectResponse(ModulePath);
            };
            Get["/settings"] = x => View["settings.cshtml", new { Title = "Ripper Settings", Settings = new Persister<RipperSettings>("OpenHomeServer").GetValue() }];
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
