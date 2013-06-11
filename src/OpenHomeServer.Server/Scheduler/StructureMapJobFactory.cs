using Quartz;
using Quartz.Spi;
using StructureMap;

namespace OpenHomeServer.Server.Jobs
{
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public StructureMapJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _container.GetInstance(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            //_container.(job);
        }
    }
}
