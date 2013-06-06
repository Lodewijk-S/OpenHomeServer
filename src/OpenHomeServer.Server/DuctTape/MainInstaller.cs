using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using OpenHomeServer.Server.Jobs;

namespace OpenHomeServer.Server.DuctTape
{
    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            //Container configuration
            container.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            //Logging            
            container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver());

            //Application parts
            container.Register(
                Component.For<OpenHomeServerService>(),
                Component.For<JobOrganiser>(),
                Component.For<OwinWebApplicationHost>()
            );
        }
    }
}