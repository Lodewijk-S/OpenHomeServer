using System;
using OpenHomeServer.Server.DuctTape;
using Topshelf;
using Serilog;
using System.Reflection;
using System.Net;

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
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null && e.InnerException is HttpListenerException) 
                {
                    _logger.Error(e, "Unable to start ServiceHost. You need to run as Administrator if you want to open up to more than localhost.");
                }
                else
                {
                    _logger.Error(e, "Unable to start ServiceHost");
                }
                
                return false;
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
