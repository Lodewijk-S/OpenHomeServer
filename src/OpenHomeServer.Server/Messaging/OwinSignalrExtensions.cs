using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using StructureMap;

namespace OpenHomeServer.Server.Messaging
{
    public static class OwinSignalrExtensions
    {
        public static IAppBuilder UseSignalr(this IAppBuilder app, IContainer container)
        {
            var resolver = container.GetInstance<StructureMapDependencyResolver>();
            var hubPipeline = resolver.Resolve<IHubPipeline>();            

            var config = new HubConfiguration
            {
                EnableDetailedErrors = true,
                Resolver = resolver
            };

            //SignalR
            app.Properties["host.AppName"] = "OpenHomeServer.Server"; //https://github.com/SignalR/SignalR/issues/1616
            foreach (var m in container.GetAllInstances<IHubPipelineModule>())
            {
                hubPipeline.AddModule(m);
            }

            app.MapHubs(config);

            return app;
        }
    }
}
