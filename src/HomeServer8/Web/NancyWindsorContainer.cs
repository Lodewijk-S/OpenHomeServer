using Castle.Windsor;
using Nancy.Bootstrappers.Windsor;

namespace HomeServer8.Server.Web
{
    public class MyWindsorNancyBootstrapper : WindsorNancyBootstrapper
    {
        private static IWindsorContainer _container;

        public static void SetApplicationContainer(IWindsorContainer container)
        {
            _container = container;
        }

        protected override Castle.Windsor.IWindsorContainer GetApplicationContainer()
        {
            return _container;
        }
    }
}
