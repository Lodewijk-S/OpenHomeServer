using Castle.MicroKernel.Registration;
using Quartz;
using Quartz.Spi;

namespace OpenHomeServer.Server.Jobs
{
    public class JobInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IJobFactory>().ImplementedBy<WindsorJobFactory>(),
                Classes.FromThisAssembly().BasedOn<IJob>()
            );
        }
    }
}
