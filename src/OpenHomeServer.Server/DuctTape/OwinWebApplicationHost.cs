using OpenHomeServer.Server.Messaging;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using Castle.Windsor;
using Serilog.Extras.MSOwin;
using Serilog;

namespace OpenHomeServer.Server.DuctTape
{
    public class OwinWebApplicationHost : IDisposable
    {
        IDisposable _host;
        readonly IWindsorContainer _container;
        readonly ILogger _logger;

        public OwinWebApplicationHost(IWindsorContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public void Start()
        {
            var startOptions = new StartOptions
            {
                Port = 8083
            };

            _host = WebApp.Start(startOptions, a =>
            {
                //SignalR
                a.UseSignalr(_container);

                //Nancy
                a.UseNancy();

                //Serilog
                a.UseSerilogRequestContext();
                a.UseSerilog(_logger);

                var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, Object>>;
                _logger.Information("Webserver started at port {0}", addresses[0]["port"]);
            });
        }

        public void Dispose()
        {
            if (_host != null) _host.Dispose();
        }
    }
}
