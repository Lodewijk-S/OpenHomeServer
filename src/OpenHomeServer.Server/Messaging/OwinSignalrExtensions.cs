using Castle.Windsor;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Owin;

namespace OpenHomeServer.Server.Messaging
{
    public static class OwinSignalrExtensions
    {
        public static IAppBuilder UseSignalr(this IAppBuilder app, IWindsorContainer container)
        {
            var resolver = container.Resolve<WindsorDependencyResolver>();
            var hubPipeline = resolver.Resolve<IHubPipeline>();            

            var config = new HubConfiguration
            {
                EnableDetailedErrors = true,
                Resolver = resolver
            };

            //SignalR
            app.Properties["host.AppName"] = "OpenHomeServer.Server"; //https://github.com/SignalR/SignalR/issues/1616
            foreach (var m in container.ResolveAll<IHubPipelineModule>())
            {
                hubPipeline.AddModule(m);
            }

            app.MapHubs(config);

            return app;
        }
    }
}
