using Nancy;
using Nancy.Bootstrapper;
using Serilog;
using Serilog.Core;

namespace OpenHomeServer.Server.Web
{
    public static class NancyWithSerilog
    {
        public static void WithSerilog(this IPipelines pipelines, ILogger logger = null)
        {
            logger = logger ?? Log.ForContext(Constants.SourceContextPropertyName, "Nancy");

            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                if (ctx == null || ctx.Response == null)
                    return;

                logger.Information("Nancy processed a request: {@NancyContext}", new NancyContextLog(ctx));
            });

            pipelines.OnError += (ctx, ex) =>
            {
                logger.Error(ex, "Nancy failed to process this request: {@NancyContext}", new NancyContextLog(ctx));
                return null;
            };
        }

        public class NancyContextLog
        {
            public string Url { get; set; }
            public string StatusCode { get; set; }
            public string ModuleName { get; set; }
            public string ModulePath { get; set; }
            public string UserName { get; set; }

            public NancyContextLog(NancyContext context)
            {
                Url = context.Request == null ? string.Empty : context.Request.Url.ToString();
                StatusCode = context.Response == null ? string.Empty : context.Response.StatusCode.ToString();
                ModuleName = context.NegotiationContext == null ? string.Empty : context.NegotiationContext.ModuleName;
                ModulePath = context.NegotiationContext == null ? string.Empty : context.NegotiationContext.ModulePath;
                UserName = context.CurrentUser == null ? string.Empty : context.CurrentUser.UserName;
            }
        }
    }
}