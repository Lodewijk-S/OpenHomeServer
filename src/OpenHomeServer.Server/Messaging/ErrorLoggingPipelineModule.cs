using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using System;

namespace OpenHomeServer.Server.Messaging
{
    public class ErrorLoggingPipelineModule : HubPipelineModule
    {
        private readonly ILogger _logger;

        public ErrorLoggingPipelineModule(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            _logger.Error("Error in Signalr", exceptionContext.Error);
        }
    }
}
