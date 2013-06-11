using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StructureMap.Configuration.DSL;

namespace OpenHomeServer.Server.Jobs
{
    public class JobRegistry : Registry
    {
        public JobRegistry()
        {
            For<IJobFactory>().Use<StructureMapJobFactory>();
            For<IScheduler>().Use(c => {
                var schedFact = new StdSchedulerFactory();
                var scheduler = schedFact.GetScheduler();
                scheduler.JobFactory = c.GetInstance<IJobFactory>();
                return scheduler;
            });

            Scan(x => {
                x.TheCallingAssembly();
                x.AddAllTypesOf<IJobDefinition>();
                x.WithDefaultConventions();
            });

            Scan(x => {
                x.TheCallingAssembly();
                x.AddAllTypesOf<IJob>();                
            });
        }
    }
}
