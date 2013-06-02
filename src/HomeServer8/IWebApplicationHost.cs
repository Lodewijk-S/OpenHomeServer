using HomeServer8.Server.Messaging;
using Microsoft.AspNet.SignalR;
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

        public OwinWebApplicationHost(WindsorDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Start()
        {
            _host = WebApplication.Start(8080, a =>
            {
                var config = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    Resolver = _resolver
                };
                //a.MapHubs(config);
                //a.UseNancy();

                var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, System.Object>>;
                Console.WriteLine("Webserver started at port {0}", addresses[0]["port"]);
            });
        }

        public void Dispose()
        {
            if (_host != null) _host.Dispose();
        }
    }
}
