using Microsoft.AspNet.SignalR;
using System;

namespace HomeServer8.Server.Messaging.Hubs
{
    public class MessagingHub : Hub
    {
        public void SendMessage(string message)
        {
            //throw new Exception("test");
            Clients.All.showMessage(message);
        }
    }
}
