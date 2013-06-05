using Common.Logging;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Messaging
{
    public class ErrorLoggingPipelineModule : HubPipelineModule
    {
        private ILog _logger;

        public ErrorLoggingPipelineModule(ILog logger)
        {
            _logger = logger;
        }

        protected override void OnIncomingError(Exception ex, IHubIncomingInvokerContext context)
        {
            base.OnIncomingError(ex, context);
            _logger.Error("Error in Signalr", ex);
        }
    }
}
