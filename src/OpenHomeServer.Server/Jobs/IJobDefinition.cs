using Quartz;
using System;

namespace OpenHomeServer.Server.Jobs
{
    public interface IJobDefinition
    {
        Type GetJobType();

        ITrigger GetDefaultTrigger();
    }

    public abstract class JobDefinition<T> : IJobDefinition
        where T : IJob
    {
        public Type GetJobType()
        {
            return typeof(T);
        }

        public abstract ITrigger GetDefaultTrigger();
    }

    public class TestJobDefinition : JobDefinition<TestJob>
    {
        public override ITrigger GetDefaultTrigger()
        {
            return TriggerBuilder
                .Create()
                .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).RepeatForever())
                .StartNow()
                .Build();
        }
    }
}
