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
            var startOptions = new StartOptions
            {
                Port = 8080
            };

            _host = WebApplication.Start(startOptions, a =>
            {
                try
                {
                    var config = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        Resolver = _resolver
                    };
                    
                    //SignalR
                    a.Properties["host.AppName"] = "Homeserver8.Server"; //https://github.com/SignalR/SignalR/issues/1616
                    a.MapHubs(config);

                    //Nancy
                    a.UseNancy();

                    var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, System.Object>>;
                    Console.WriteLine("Webserver started at port {0}", addresses[0]["port"]);
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }

        public void Dispose()
        {
            if (_host != null) _host.Dispose();
        }
    }
}
