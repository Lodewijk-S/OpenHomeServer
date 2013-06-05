using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR.Hubs;

namespace HomeServer8.Server.Messaging
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<WindsorDependencyResolver>(),
                Classes.FromThisAssembly().BasedOn<IHubPipelineModule>().WithServiceFromInterface()
            );
        }
    }
}
