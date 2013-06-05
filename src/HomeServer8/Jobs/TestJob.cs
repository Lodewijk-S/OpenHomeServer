using HomeServer8.Server.Messaging;
using HomeServer8.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Quartz;

namespace HomeServer8.Server.Jobs
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
}
