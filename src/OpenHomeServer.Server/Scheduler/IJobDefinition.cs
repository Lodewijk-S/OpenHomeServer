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
}
