using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Hubs
{
    public class MessagingHub : Hub
    {
        public void SendMessage(string message)
        {
            Clients.All.showMessage(message);
        }
    }
}
