using Common.Logging;
using System;
using OpenHomeServer.Server.DuctTape;
using Topshelf;

namespace OpenHomeServer.Server
{
    public class OpenHomeServerService : ServiceControl
    {
        private readonly OwinWebApplicationHost _webHost;
        private readonly Jobs.JobOrganiser _jobOrganiser;
        private readonly ILog _logger;

        public OpenHomeServerService(OwinWebApplicationHost webHost, Jobs.JobOrganiser jobOrganiser, ILog logger)
        {
            _webHost = webHost;
            _jobOrganiser = jobOrganiser;
            _logger = logger;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _webHost.Start();
                _jobOrganiser.Start();
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
                if (_jobOrganiser != null) _jobOrganiser.Dispose();
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
