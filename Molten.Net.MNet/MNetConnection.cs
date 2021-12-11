using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    public class MNetConnection : INetworkConnection, IDisposable
    {
        public ConnectionStatus Status { get; internal set; }
        public string Host { get; }
        public int Port { get; }

        internal IPEndPoint Endpoint { get; }
        internal Socket TCPSocket { get; }
        internal Socket UDPSocket { get; }

        internal MNetConnection(string host, int port)
        {
            Host = host;
            Port = port;
            Status = ConnectionStatus.Disconnected;

            //TODO Address validation
            Endpoint = new IPEndPoint(new IPAddress(Host.Split('.').Select(x => byte.Parse(x)).ToArray()), Port);
            TCPSocket = new Socket(Endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            UDPSocket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Dispose()
        {
            TCPSocket.Dispose();
            UDPSocket.Dispose();
        }
    }
}
