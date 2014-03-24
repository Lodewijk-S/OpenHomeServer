using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;
using OpenHomeServer.Server.Plugins.Notifications;

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
                Component.For<Notificator>().UsingFactoryMethod(k => new Notificator(k.Resolve<WindsorDependencyResolver>().Resolve<IConnectionManager>()))
            );
        }
    }
}
