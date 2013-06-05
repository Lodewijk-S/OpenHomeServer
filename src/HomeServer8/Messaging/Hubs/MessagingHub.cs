using Common.Logging;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace HomeServer8.Server.Messaging.Hubs
{
    public class MessagingHub : Hub
    {
        ILog _logger;

        public MessagingHub(ILog logger)
        {
            _logger = logger;
        }

        public void SendMessage(string message)
        {
            Clients.SendMessage(message);
        }
    }

    public static class MessagingHubExtensions
    {
        public static void SendMessage(this IHubConnectionContext clients, string message)
        {
            clients.All.showMessage(message);
        }
    }
}