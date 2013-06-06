using OpenHomeServer.Server.Messaging;
using OpenHomeServer.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Quartz;

namespace OpenHomeServer.Server.Jobs
{
    public class TestJob : IJob
    {
        IHubContext _context;

        public TestJob(HubContextFactory<MessagingHub> factory)
        {
            _context = factory.GetContext();
        }

        public void Execute(IJobExecutionContext context)
        {
            _context.Clients.SendMessage("Ping From Job");
        }
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
