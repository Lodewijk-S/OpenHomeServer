using Common.Logging;
using System;
using Topshelf;

namespace HomeServer8.Server.Bootstrappers
{
    public class ServiceBootstrapper : ServiceControl
    {
        private IWebApplicationHost _webHost;
        private ISchedulerHost _scheduler;
        private ILog _logger;

        public ServiceBootstrapper(IWebApplicationHost webHost, ISchedulerHost scheduler, ILog logger)
        {
            _webHost = webHost;
            _scheduler = scheduler;
            _logger = logger;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _webHost.Start();
                _scheduler.Start();
                return true;
            }
            catch(Exception e)
            {
                _logger.Error("Unable to start ServiceHost", e);
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                if (_scheduler != null) _scheduler.Dispose();
                if (_webHost != null) _webHost.Dispose();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Unable to stop ServiceHost", e);
                return false;
            }
        }        
    }
}
