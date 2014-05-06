using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace OpenHomeServer.Server.Plugins.Notifications
{
    public class NotificationHub : Hub
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly ILogger _logger;

        public NotificationHub(NotificationRepository notificationRepository, ILogger logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public void NotifyAll(Notification notification)
        {
            _notificationRepository.AddNotification(notification);
            SendToLog(notification);
            Clients.NotifyAll(notification);
        }

        private void SendToLog(Notification notification)
        {
            switch (notification.Level)
            {
                case Level.Info:
                    _logger.Information("A notification has been posted: {message} ({link})", notification.Message, notification.Link);
                    break;
                case Level.Warning:
                    _logger.Warning("A notification has been posted: {message} ({link})", notification.Message, notification.Link);
                    break;
                case Level.Error:
                    _logger.Error("A notification has been posted: {message} ({link})", notification.Message, notification.Link);
                    break;
            }
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