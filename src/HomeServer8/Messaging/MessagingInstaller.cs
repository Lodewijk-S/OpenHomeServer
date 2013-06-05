using Castle.MicroKernel.Registration;
using HomeServer8.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace HomeServer8.Server.Messaging
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<WindsorDependencyResolver>(),
                Classes.FromThisAssembly().BasedOn<IHubPipelineModule>().WithServiceFromInterface(),
                Classes.FromThisAssembly().BasedOn<IHub>(),
                Component.For(typeof(HubContextFactory<>))
            );
        }
    }

    public class HubContextFactory<T> where T : Hub
    {
        IConnectionManager _connectionManager;

        public HubContextFactory(WindsorDependencyResolver resolver)
        {
            _connectionManager = resolver.Resolve<IConnectionManager>();
        }

        public IHubContext GetContext()
        {
            return _connectionManager.GetHubContext<T>();
        }
    }
}
