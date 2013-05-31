using HomeServer8.Server.Messaging.Hubs;
using Nancy.TinyIoc;
using Quartz;
using Quartz.Impl;
using System;

namespace HomeServer8.Server.Jobs
{
    public class QuartzBootstrapper : IDisposable
    {
        IScheduler _sched;

        public QuartzBootstrapper()
        {
            var schedFact = new StdSchedulerFactory();

            _sched = schedFact.GetScheduler();
            _sched.Start();

            
            var jobDetail = JobBuilder
                .Create<TestJob>()
                .Build();
            
            var trigger = TriggerBuilder
                .Create()
                .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).RepeatForever())
                .StartNow()
                .Build();
            
            _sched.ScheduleJob(jobDetail, trigger);
        }

        public void Dispose()
        {
            if (_sched != null) _sched.Shutdown();
        }
    }

    public class TestJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            TinyIoCContainer.Current.Resolve<MessagingHub>().SendMessage("Hello!");
        }
    }
}
