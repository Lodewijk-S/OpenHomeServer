using Castle.Windsor;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Jobs
{
    public class WindsorJobFactory : IJobFactory
    {
        private IWindsorContainer _container;

        public WindsorJobFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public Quartz.IJob NewJob(TriggerFiredBundle bundle, Quartz.IScheduler scheduler)
        {
            return _container.Resolve(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(Quartz.IJob job)
        {
            _container.Release(job);
        }
    }
}
