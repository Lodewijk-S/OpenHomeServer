using Nancy;

namespace OpenHomeServer.Server.Plugins.Notifications
{
    public class NotificationModule : NancyModule
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationModule(NotificationRepository notificationRepository)
            : base("notification")
        {
            _notificationRepository = notificationRepository;

            Get["/"] = x => View["index.cshtml", new { Notifications = _notificationRepository.GetLatestNotifications() }];
        }
    }
}
