using System;
using Topshelf;

namespace HomeServer8.Server.Bootstrappers
{
    public class ServiceBootstrapper : ServiceControl
    {
        public IWebApplicationHost _webHost;
        public ISchedulerHost _scheduler;

        public ServiceBootstrapper(IWebApplicationHost webHost, ISchedulerHost scheduler)
        {
            _webHost = webHost;
            _scheduler = scheduler;
        }

        public bool Start(HostControl hostControl)
        {
            _webHost.Start();
            _scheduler.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_scheduler != null) _scheduler.Dispose();
            if (_webHost != null) _webHost.Dispose();

               
            return true;
        }        
    }
}
