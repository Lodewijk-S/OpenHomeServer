using HomeServer8.Server.Jobs;
using Quartz;
using Quartz.Impl;
using System;

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
            var jobDetail = JobBuilder
                .Create<TestJob>()
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).RepeatForever())
                .StartNow()
                .Build();

            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Shutdown();
        }
    }
}
