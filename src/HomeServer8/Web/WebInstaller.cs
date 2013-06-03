using Castle.MicroKernel.Registration;
using Nancy.Bootstrappers.Windsor;

namespace HomeServer8.Server.Web
{
    public class WebInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            //A bit hackish, but this is how Nancy likes it
            MyWindsorNancyBootstrapper.SetApplicationContainer(container);

            container.Register(
                Component.For<NancyRequestScopeInterceptor>()               
            );

            container.Kernel.ProxyFactory.AddInterceptorSelector(new NancyRequestScopeInterceptorSelector());
        }
    }
}
