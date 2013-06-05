using HomeServer8.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Quartz;

namespace HomeServer8.Server.Jobs
{
    public class TestJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            GlobalHost.ConnectionManager.GetHubContext<MessagingHub>().Clients.All.showMessage("Ping From Job");
        }
    }
}
