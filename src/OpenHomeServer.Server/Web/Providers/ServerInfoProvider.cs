namespace OpenHomeServer.Server.Web.Providers
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
