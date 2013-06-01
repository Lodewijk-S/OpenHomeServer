using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Topshelf;

namespace HomeServer8.Server.Bootstrappers
{
    static class BootStrapper
    {
        public static void Main(string[] args)
        {
            using (var container = new WindsorContainer())
            {
                container.Install(new MainInstaller());

                HostFactory.Run(x =>
                {
                    x.SetServiceName("HomeServer8");
                    x.SetDescription("Home Server Service for Windows 8");
                    x.SetDisplayName("HomeServer8 Service");

                    x.StartAutomatically();

                    x.Service<ServiceBootstrapper>(() => container.Resolve<ServiceBootstrapper>());
                });
            }
        }        
    }

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            //Container configuration
            container.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            //Service Configuration

        }
    }
}
