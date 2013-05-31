using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.SSDP
{
    public class SSDPServer
    {
        UdpClient client = new UdpClient();
        const string SSDP_ADDR = "239.255.255.250";
        const int SSDP_PORT = 1900;
        IPEndPoint SSDP_ENDP = new IPEndPoint(IPAddress.Parse(SSDP_ADDR), SSDP_PORT);
        IPAddress SSDP_IP = IPAddress.Parse(SSDP_ADDR);

        public void Start()
        {
            client.Client.UseOnlyOverlappedIO = true;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(new IPEndPoint(IPAddress.Any, SSDP_PORT));
            client.JoinMulticastGroup(SSDP_IP, 2);

            client.BeginReceive(new AsyncCallback(OnRecieve), null);

            var message = @"M-SEARCH * HTTP/1.1\r\n" +
                           "HOST: 239.255.255.250:1900\r\n" +
                           "ST:upnp:rootdevice\r\n" +
                           "MAN:\"ssdp:discover\"\r\n" +
                           "MX:3\r\n\r\n";
            //https://github.com/nmaier/simpleDLNA/blob/master/server/Ssdp/Datagram.cs
            var msgBytes = Encoding.ASCII.GetBytes(message);
            client.Send(msgBytes, msgBytes.Length);
        }

        public void Stop()
        {
            client.DropMulticastGroup(SSDP_IP);
        }

        private void OnRecieve(IAsyncResult result)
        {
            var endpoint = new IPEndPoint(IPAddress.None, SSDP_PORT);
            var received = client.EndReceive(result, ref endpoint);

            using (var reader = new StreamReader(new MemoryStream(received), Encoding.ASCII))
            {

            }
        }
    }
}
