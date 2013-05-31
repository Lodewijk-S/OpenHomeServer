using HomeServer8.Server.Hubs;
using HomeServer8.Server.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using Topshelf;
using Castle.Windsor;

namespace HomeServer8.Server.Bootstrappers
{
    public class ServiceBootstrapper : ServiceControl
    {
        IDisposable _app;
        IDisposable _scheduler;

        public bool Start(HostControl hostControl)
        {
            try
            {
               

                //_app = CreateOwinHost();
                _scheduler = new QuartzBootstrapper();

                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            

            if (_app != null) _app.Dispose();
            if (_scheduler != null) _scheduler.Dispose();            
            return true;
        }

        private IDisposable CreateOwinHost()
        {
            return WebApplication.Start(8080, a =>
            {
                TinyIoCContainer.Current.Register<MessagingHub>();

                var config = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    Resolver = new TinyIoCDependencyResolver(TinyIoCContainer.Current)
                };
                a.MapHubs(config);
                a.UseNancy();

                var addresses = a.Properties["host.Addresses"] as List<IDictionary<String, System.Object>>;
                Console.WriteLine("Webserver started at port {0}", addresses[0]["port"]);
            });
        }
    }
}
