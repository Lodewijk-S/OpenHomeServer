using Common.Logging;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace OpenHomeServer.Server.Messaging
{
    public class ErrorLoggingPipelineModule : HubPipelineModule
    {
        private readonly ILog _logger;

        public ErrorLoggingPipelineModule(ILog logger)
        {
            _logger = logger;
        }

        protected override void OnIncomingError(Exception ex, IHubIncomingInvokerContext context)
        {
            //base.OnIncomingError(ex, context);
            _logger.Error("Error in Signalr", ex);
        }
    }
}
