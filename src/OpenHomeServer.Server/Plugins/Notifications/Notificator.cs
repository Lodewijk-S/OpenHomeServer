using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace OpenHomeServer.Server.Plugins.Notifications
{
    public class Notificator
    {
        private readonly IHubContext _hubContext;

        public Notificator(IConnectionManager connectionManager)
        {
            _hubContext = connectionManager.GetHubContext<NotificationHub>();
        }

        public void SendNotificationToAllClients(Notification notification)
        {
            _hubContext.Clients.NotifyAll(notification);
        }
    }
}