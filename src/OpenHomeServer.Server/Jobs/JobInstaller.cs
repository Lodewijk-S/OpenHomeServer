using Castle.MicroKernel.Registration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace OpenHomeServer.Server.Jobs
{
    public class JobInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IJobFactory>().ImplementedBy<WindsorJobFactory>(),
                Component.For<IScheduler>().UsingFactoryMethod((k, c) =>
                    {
                        var schedFact = new StdSchedulerFactory();
                        var scheduler = schedFact.GetScheduler();
                        scheduler.JobFactory = container.Resolve<IJobFactory>();
                        return scheduler;
                    }),
                Classes.FromThisAssembly().BasedOn<IJobDefinition>().WithServiceBase(),
                Classes.FromThisAssembly().BasedOn<IJob>()
            );
        }
    }
}
