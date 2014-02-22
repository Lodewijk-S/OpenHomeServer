using Castle.MicroKernel.Registration;
using OpenHomeServer.Server.Web.Providers;

namespace OpenHomeServer.Server.Web
{
    public class WebInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            NancyBootstrapper.SetApplicationContainer(container);

            container.Register(                
                Component.For<ServerInfoProvider>().LifestyleTransient()
            );            
        }
    }
}
