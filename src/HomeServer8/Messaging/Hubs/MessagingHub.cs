using Microsoft.AspNet.SignalR;

namespace HomeServer8.Server.Messaging.Hubs
{
    public class MessagingHub : Hub
    {
        public void SendMessage(string message)
        {
            Clients.All.showMessage(message);
        }
    }
}
