using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        internal Dictionary<int, uint> InboundSequenceNumbers { get; }
        internal Dictionary<int, uint> _outboundSequenceNumbers;

        internal ManualResetEvent UDPWaitHandle { get; }
        internal ManualResetEvent TCPWaitHandle { get; }

        private MNetConnection(IPEndPoint endpoint)
        {
            Host = endpoint.Address.ToString();
            Port = endpoint.Port;
            InboundSequenceNumbers = new Dictionary<int, uint>();
            _outboundSequenceNumbers = new Dictionary<int, uint>();
            
            Status = ConnectionStatus.Disconnected;
            Endpoint = endpoint;
            UDPSocket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            TCPSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            UDPWaitHandle = new ManualResetEvent(true);
            TCPWaitHandle = new ManualResetEvent(true);
        }


        internal MNetConnection(string host, int port)
            : this(new IPEndPoint(new IPAddress(host.Split('.').Select(x => byte.Parse(x)).ToArray()), port))
        {

        }

        internal MNetConnection(IPAddress address, int port)
            : this(new IPEndPoint(address, port))
        {
            
        }

        internal uint GetOutboundSequenceNumber(int channel)
        {
            if (_outboundSequenceNumbers.ContainsKey(channel) == false)
            {
                _outboundSequenceNumbers[channel] = 1;
                return 0;
            }

            return _outboundSequenceNumbers[channel]++;
        }

        public void Dispose()
        {
            TCPSocket.Dispose();
            UDPSocket.Dispose();
        }
    }
}
