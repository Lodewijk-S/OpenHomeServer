using Common.Logging;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace OpenHomeServer.Server.Messaging.Hubs
{
    public class NotificationHub : Hub
    {
        ILog _logger;

        public NotificationHub(ILog logger)
        {
            _logger = logger;
        }

        public void NotifyAll(Notification notification)
        {
            Clients.NotifyAll(notification);
        }
    }

    public class NotificationService
    {
        private readonly IHubContext _hubContext;

        public NotificationService(IConnectionManager connectionManager)
        {
            _hubContext = connectionManager.GetHubContext<NotificationHub>(); ;
        }

        public void SendNotificationToAllClients(Notification notification)
        {
            _hubContext.Clients.NotifyAll(notification);
        }
    }

    public static class MessagingHubExtensions
    {
        public static void NotifyAll(this IHubConnectionContext clients, Notification notification)
        {
            clients.All.onRecieveNotification(notification);
        }
    }

    [Serializable]
    public class Notification
    {
        public string Message { get; private set; }
        public Uri Link { get; private set; }

        public Notification(string message, Uri link = null)
        {
            Link = link;
            Message = message;
        }
    }
}