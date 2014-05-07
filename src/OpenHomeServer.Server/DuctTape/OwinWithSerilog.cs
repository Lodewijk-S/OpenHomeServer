using System;
using System.Diagnostics;
using Microsoft.Owin.Logging;
using Owin;
using ISerilogLogger = Serilog.ILogger;

namespace OpenHomeServer.Server.DuctTape
{
    public static class OwinWithSerilog
    {
        public static void UseSerilog(this IAppBuilder app, ISerilogLogger logger)
        {
            app.SetLoggerFactory(new SerilogLoggerFactory(logger));
        }
    }

    public class SerilogLoggerFactory : ILoggerFactory
    {
        private readonly ISerilogLogger _logger;

        public SerilogLoggerFactory(ISerilogLogger logger)
        {
            _logger = logger;
        }

        public ILogger Create(string name)
        {
            return new SerilogLogger(_logger);
        }
    }

    public class SerilogLogger : ILogger
    {
        private readonly ISerilogLogger _logger;
        private const string SerilogMessage = "Owin had something to say: {@OwinContext}";

        public SerilogLogger(ISerilogLogger logger)
        {
            _logger = logger;
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var log = new OwinContextLog(eventId, formatter(state, exception));

            switch (eventType)
            {
                case TraceEventType.Critical:
                    _logger.Fatal(exception, SerilogMessage, log);
                    return true;
                case TraceEventType.Error:
                    _logger.Error(exception, SerilogMessage, log);
                    return true;
                case TraceEventType.Information:
                    _logger.Information(exception, SerilogMessage, log);
                    return true;
                case TraceEventType.Warning:
                    _logger.Warning(exception, SerilogMessage, log);
                    return true;
                case TraceEventType.Verbose:
                    _logger.Verbose(exception, SerilogMessage, log);
                    return true;
                default:
                    return false;
            }
        }
    }

    public class OwinContextLog
    {
        public OwinContextLog(int eventId, string message)
        {
            EventId = eventId;
            Message = message;
        }

        public string Message { get; private set; }
        public int EventId { get; private set; }
    }
}