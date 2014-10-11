using Microsoft.AspNet.SignalR;
using OpenHomeServer.Server.Messaging;

namespace OpenHomeServer.Server.Plugins.Notification
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