using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace OpenHomeServer.Server.Jobs
{
    public class JobModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobFactory>().To<NinjectJobFactory>();
            Bind<IScheduler>().ToMethod(c => {
                var schedFact = new StdSchedulerFactory();
                var scheduler = schedFact.GetScheduler();
                scheduler.JobFactory = c.Kernel.Get<IJobFactory>();
                return scheduler;
            });

            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().InheritedFrom<IJobDefinition>().BindSingleInterface());
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().InheritedFrom<IJob>());
        }
    }
}
