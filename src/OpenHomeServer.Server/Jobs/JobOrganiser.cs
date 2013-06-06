using System;
using Quartz;

namespace OpenHomeServer.Server.Jobs
{
    public class JobOrganiser : IDisposable
    {
        readonly IScheduler _scheduler;

        public JobOrganiser(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Start()
        {
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
