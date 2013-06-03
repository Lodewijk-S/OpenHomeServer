using HomeServer8.Server.Messaging.Hubs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Jobs
{
    public class TestJob : IJob
    {
        MessagingHub _hub;

        public TestJob(MessagingHub hub)
        {
            _hub = hub;
        }

        public void Execute(IJobExecutionContext context)
        {
            _hub.SendMessage("Hello!");
        }
    }
}
