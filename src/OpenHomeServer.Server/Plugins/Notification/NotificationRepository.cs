using System.Collections.Generic;

namespace OpenHomeServer.Server.Plugins.Notification
{
    public class NotificationRepository
    {
        readonly Queue<Notification> _notifications = new Queue<Notification>();

        public IEnumerable<Notification> GetLatestNotifications()
        {
            return _notifications.ToArray();
        }

        public void AddNotification(Notification notification)
        {
            if (_notifications.Count >= 10)
            {
                _notifications.Dequeue();
            }
            _notifications.Enqueue(notification);
        }
    }
}
