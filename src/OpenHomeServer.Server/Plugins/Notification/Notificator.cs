using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using OpenHomeServer.Server.Messaging;

namespace OpenHomeServer.Server.Plugins.Notifications
{
    public class Notificator
    {
        private readonly IHubContext _hubContext;

        public Notificator(IHubContextFactory hubContextFactory)
        {
            _hubContext = hubContextFactory.CreateHubContext<NotificationHub>();
        }

        public void SendNotificationToAllClients(Notification notification)
        {
            _hubContext.Clients.NotifyAll(notification);
        }
    }
}