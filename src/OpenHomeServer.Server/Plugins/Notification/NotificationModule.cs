using Nancy;

namespace OpenHomeServer.Server.Plugins.Notification
{
    public class NotificationModule : NancyModule
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationModule(NotificationRepository notificationRepository)
            : base("notifications")
        {
            _notificationRepository = notificationRepository;

            Get["/"] = x => View["index.cshtml", new { Title="Notifications", Notifications = _notificationRepository.GetLatestNotifications() }];
        }
    }
}
