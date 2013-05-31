using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Providers
{
    public class ServerInfoProvider
    {
        public ServerInfo GetServerInfo()
        {
            return new ServerInfo
            {
                ServerName = System.Net.Dns.GetHostName()
            };
        }

        public class ServerInfo
        {
            public string ServerName { get; set; }
        }
    }
}
