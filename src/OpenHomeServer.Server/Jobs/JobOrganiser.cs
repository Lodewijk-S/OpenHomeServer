using System;
using Quartz;
using System.Collections.Generic;

namespace OpenHomeServer.Server.Jobs
{
    public class JobOrganiser : IDisposable
    {
        readonly IScheduler _scheduler;
        readonly IEnumerable<IJobDefinition> _jobdefinitions;

        public JobOrganiser(IScheduler scheduler, IEnumerable<IJobDefinition> jobDefinitions)
        {
            _scheduler = scheduler;
            _jobdefinitions = jobDefinitions;
        }

        public void Start()
        {
            foreach (var j in _jobdefinitions)
            {
                var jobDetail = JobBuilder.Create(j.GetJobType()).Build();
                var trigger = j.GetDefaultTrigger();
                _scheduler.ScheduleJob(jobDetail, trigger);
            }

            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Shutdown();
        }
    }
}
