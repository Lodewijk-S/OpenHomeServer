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

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            _logger.Error("Error in Signalr", exceptionContext.Error);
        }
    }
}
