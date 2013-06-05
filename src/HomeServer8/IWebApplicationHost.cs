using Common.Logging;
using HomeServer8.Server.Messaging;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;

namespace HomeServer8.Server
{
    public interface IWebApplicationHost : IDisposable
    {
        void Start();
    }

    public class OwinWebApplicationHost : IWebApplicationHost
    {
        IDisposable _host;
        WindsorDependencyResolver _resolver;
        ILog _logger;

        public OwinWebApplicationHost(WindsorDependencyResolver resolver, ILog logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public void Start()
        {
            var startOptions = new StartOptions
            {
                Port = 8080
            };

            _host = WebApplication.Start(startOptions, a =>
            {
                var config = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    Resolver = _resolver
                };

                //SignalR
                a.Properties["host.AppName"] = "Homeserver8.Server"; //https://github.com/SignalR/SignalR/issues/1616
                a.MapHubs(config);
                foreach (var m in _resolver.ResolveAll<IHubPipelineModule>())
                {
                    GlobalHost.HubPipeline.AddModule(m);
                }


                //Nancy
                a.UseNancy();

                var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, System.Object>>;
                _logger.InfoFormat("Webserver started at port {0}", addresses[0]["port"]);
            });
        }

        public void Dispose()
        {
            if (_host != null) _host.Dispose();
        }
    }
}
