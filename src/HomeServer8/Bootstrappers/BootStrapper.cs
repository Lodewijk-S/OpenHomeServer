using Castle.Windsor;
using Topshelf;

namespace HomeServer8.Server.Bootstrappers
{
    static class BootStrapper
    {
        public static void Main(string[] args)
        {
            IWindsorContainer _container = null;

            try
            {
                _container = new WindsorContainer();

                HostFactory.Run(x =>
                {
                    x.SetServiceName("HomeServer8");
                    x.SetDescription("Home Server Service for Windows 8");
                    x.SetDisplayName("HomeServer8 Service");

                    x.StartAutomatically();

                    x.Service<ServiceBootstrapper>();
                });
            }
            finally
            {
                if (_container != null) _container.Dispose();
            }
        }        
    }
}
