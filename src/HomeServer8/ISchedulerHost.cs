using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeServer8.Server
{
    public interface ISchedulerHost : IDisposable
    {
        void Start();
    }

    public class QuartzScheduler : ISchedulerHost
    {
        IScheduler _scheduler;

        public void Start()
        {
            var schedFact = new StdSchedulerFactory();

            _scheduler = schedFact.GetScheduler();

            //todo: get the jobs from the container

            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Shutdown();
        }
    }
}
