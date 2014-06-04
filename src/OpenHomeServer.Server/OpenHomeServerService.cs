using System;
using OpenHomeServer.Server.DuctTape;
using Topshelf;
using Serilog;

namespace OpenHomeServer.Server
{
    public class OpenHomeServerService : ServiceControl
    {
        private readonly OwinWebApplicationHost _webHost;
        private readonly ILogger _logger;

        public OpenHomeServerService(OwinWebApplicationHost webHost, ILogger logger)
        {
            _webHost = webHost;
            _logger = logger;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _webHost.Start();
                return true;
            }
            catch(Exception e)
            {
                _logger.Error(e, "Unable to start ServiceHost");
                return false;
            }
        }
        
        public bool Stop(HostControl hostControl)
        {
            try
            {
                if (_webHost != null) _webHost.Dispose();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unable to stop ServiceHost");
                return false;
            }
        }        
    }
}
