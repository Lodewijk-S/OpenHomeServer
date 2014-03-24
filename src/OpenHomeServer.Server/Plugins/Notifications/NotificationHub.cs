using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace OpenHomeServer.Server.Plugins.Notifications
{
    public class NotificationHub : Hub
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationHub(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void NotifyAll(Notification notification)
        {
            _notificationRepository.AddNotification(notification);
            Clients.NotifyAll(notification);
        }
    }

    public static class MessagingHubExtensions
    {
        public static void NotifyAll(this IHubConnectionContext clients, Notification notification)
        {
            clients.All.onRecieveNotification(notification);
        }
    }
}