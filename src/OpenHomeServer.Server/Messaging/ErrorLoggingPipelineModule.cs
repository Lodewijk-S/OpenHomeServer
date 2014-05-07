using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

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
            _logger.Error(exceptionContext.Error, "Signalr encountered an error: {@SignalrContext}", new SignalrContextLog(invokerContext));
        }
    }

    public class SignalrContextLog
    {
        public SignalrContextLog(IHubIncomingInvokerContext context)
        {
            HubName = context.MethodDescriptor.Hub.Name;
            MethodName = context.MethodDescriptor.Name;
        }

        public string HubName { get; private set; }
        public string MethodName { get; private set; }
    }
}
