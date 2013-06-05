using OpenHomeServer.Server.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace OpenHomeServer.Server
{
    public interface ISchedulerHost : IDisposable
    {
        void Start();
    }

    public class QuartzScheduler : ISchedulerHost
    {
        IScheduler _scheduler;
        IJobFactory _jobFactory;

        public QuartzScheduler(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public void Start()
        {
            var schedFact = new StdSchedulerFactory();
            _scheduler = schedFact.GetScheduler();
            _scheduler.JobFactory = _jobFactory;

            //todo: get the jobs from the container
            var jobDetail = JobBuilder
                .Create<TestJob>()
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).RepeatForever())
                .StartNow()
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Shutdown();
        }
    }
}
