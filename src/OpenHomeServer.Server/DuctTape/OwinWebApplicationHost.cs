using Common.Logging;
using OpenHomeServer.Server.Messaging;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using Castle.Windsor;

namespace OpenHomeServer.Server.DuctTape
{
    public class OwinWebApplicationHost : IDisposable
    {
        IDisposable _host;
        readonly IWindsorContainer _container;
        readonly ILog _logger;

        public OwinWebApplicationHost(IWindsorContainer container, ILog logger)
        {
            _container = container;
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
                //SignalR
                a.UseSignalr(_container);

                //Nancy
                a.UseNancy();

                var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, Object>>;
                _logger.InfoFormat("Webserver started at port {0}", addresses[0]["port"]);
            });
        }

        public void Dispose()
        {
            if (_host != null) _host.Dispose();
        }
    }
}
