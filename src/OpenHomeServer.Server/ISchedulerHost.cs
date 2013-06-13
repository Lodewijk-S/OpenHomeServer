using OpenHomeServer.Server.Jobs;
using Quartz;
<<<<<<< HEAD:src/OpenHomeServer.Server/Jobs/JobOrganiser.cs
using System.Collections.Generic;
=======
using Quartz.Impl;
using Quartz.Spi;
using System;
>>>>>>> parent of bbc5271... Some restructuring:src/OpenHomeServer.Server/ISchedulerHost.cs

namespace OpenHomeServer.Server
{
    public interface ISchedulerHost : IDisposable
    {
<<<<<<< HEAD:src/OpenHomeServer.Server/Jobs/JobOrganiser.cs
        readonly IScheduler _scheduler;
        readonly IEnumerable<IJobDefinition> _jobdefinitions;

        public JobOrganiser(IScheduler scheduler, IEnumerable<IJobDefinition> jobDefinitions)
        {
            _scheduler = scheduler;
            _jobdefinitions = jobDefinitions;
=======
        void Start();
    }

    public class QuartzScheduler : ISchedulerHost
    {
        IScheduler _scheduler;
        IJobFactory _jobFactory;

        public QuartzScheduler(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
>>>>>>> parent of bbc5271... Some restructuring:src/OpenHomeServer.Server/ISchedulerHost.cs
        }

        public void Start()
        {
<<<<<<< HEAD:src/OpenHomeServer.Server/Jobs/JobOrganiser.cs
            foreach (var j in _jobdefinitions)
            {
                var jobDetail = JobBuilder.Create(j.GetJobType()).Build();
                var trigger = j.GetDefaultTrigger();
                _scheduler.ScheduleJob(jobDetail, trigger);
            }

=======
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
>>>>>>> parent of bbc5271... Some restructuring:src/OpenHomeServer.Server/ISchedulerHost.cs
            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Shutdown();
        }
    }
}
