using Microsoft.AspNet.SignalR;
using OpenHomeServer.Server.Messaging;

namespace OpenHomeServer.Server.Plugins.Ripper
{
    public class RipperNotificationHub : Hub
    {
        
    }

    public class RipperNotificator
    {
        private readonly IHubContext _hubContext;

        public RipperNotificator(IHubContextFactory hubContextFactory)
        {
            _hubContext = hubContextFactory.CreateHubContext<RipperNotificationHub>();
        }

        public void UpdateProgress(int trackNumber, double percentageComplete)
        {
            _hubContext.Clients.All.onRippingProgress(trackNumber, percentageComplete);
        }

        public void UpdateStatus(RippingStatus status)
        {
            _hubContext.Clients.All.onStatusChanged(status);
        }
    }
}