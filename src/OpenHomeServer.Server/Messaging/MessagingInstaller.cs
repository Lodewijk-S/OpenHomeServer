using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using OpenHomeServer.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR.Hubs;

namespace OpenHomeServer.Server.Messaging
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<WindsorDependencyResolver>(),
                Classes.FromThisAssembly().BasedOn<IHubPipelineModule>().WithServiceFromInterface(),
                Classes.FromThisAssembly().BasedOn<IHub>(),
                Component.For<NotificationService>().UsingFactoryMethod(k => new NotificationService(k.Resolve<WindsorDependencyResolver>().Resolve<IConnectionManager>()))
            );
        }
    }
}
