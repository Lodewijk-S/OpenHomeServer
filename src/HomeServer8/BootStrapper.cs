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
                HostFactory.Run(x =>
                {
                    x.SetServiceName("HomeServer8");
                    x.SetDescription("Home Server Service for Windows 8");
                    x.SetDisplayName("HomeServer8 Service");

                    x.StartAutomatically();

                    x.Service<ServiceBootstrapper>();
                });
            }
        }        
    }
}
