using OpenHomeServer.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using StructureMap.Configuration.DSL;

namespace OpenHomeServer.Server.Messaging
{
    public class MessagingRegistry : Registry
    {
        public MessagingRegistry()
        {
            ForConcreteType<StructureMapDependencyResolver>();
            For(typeof(HubContextFactory<>));

            Scan(x => {
                x.TheCallingAssembly();

                x.AddAllTypesOf<IHubPipelineModule>();
                x.WithDefaultConventions();
            });

            Scan(x => {
                x.TheCallingAssembly();

                x.AddAllTypesOf<IHub>();
            });
        }
    }

    public class HubContextFactory<T> where T : Hub
    {
        IConnectionManager _connectionManager;

        public HubContextFactory(StructureMapDependencyResolver resolver)
        {
            _connectionManager = resolver.Resolve<IConnectionManager>();
        }

        public IHubContext GetContext()
        {
            return _connectionManager.GetHubContext<T>();
        }
    }
}
