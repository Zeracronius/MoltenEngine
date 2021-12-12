using Molten.Net.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    public class MNetService : NetworkService
    {
        private List<MNetConnection> _connections;
        private Socket _tcpListener;
        private Socket _udpListener;
        private Stack<byte[]> _buffers;

        public MNetService()
        {
            _connections = new List<MNetConnection>();

            _tcpListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _udpListener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _buffers = new Stack<byte[]>();
        }

        protected override void OnInitialize(EngineSettings settings, Logger log)
        {
            IPAddress localAddress = new IPAddress(settings.Network.ListeningAddress.Value.Split('.').Select(x => byte.Parse(x)).ToArray());
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, settings.Network.Port);

            _udpListener.Bind(localEndPoint);
            _tcpListener.Bind(localEndPoint);
            _tcpListener.Listen(100);

            base.OnInitialize(settings, log);
        }

        public override INetworkConnection Connect(string host, int port, byte[] data = null)
        {
            MNetConnection connection = new MNetConnection(host, port);
            connection.Status = ConnectionStatus.InitiatedConnect;
            _connections.Add(connection);
            SendTCP(data, connection);
            return connection;
        }

        public override IEnumerable<INetworkConnection> GetConnections()
        {
            return _connections;
        }

        protected override void OnUpdate(Timing time)
        {
            // Handle incoming.
            ListenUDP();
            ListenTCP();

            // Handle outgoing.
            foreach ((INetworkMessage, INetworkConnection) message in _outbox)
                Send(message.Item1, message.Item2);
        }

        private async void ListenTCP()
        {
            using (Socket connection = await _tcpListener.AcceptAsync())
            {
                EndPoint remoteEndpoint = connection.RemoteEndPoint;

                Stack<byte[]> filled = new Stack<byte[]>();
                int bytesRecieved = 0;
                int totalBytes = 0;
                byte[] buffer;
                do
                {
                    if (_buffers.Count > 0)
                        buffer = _buffers.Pop();
                    else
                        buffer = new byte[1024];

                    bytesRecieved = await connection.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), SocketFlags.None);
                    totalBytes += bytesRecieved;
                    filled.Push(buffer);
                }
                while (buffer[bytesRecieved] != 0x1A);

                connection.Disconnect(true);

                byte[] data = new byte[totalBytes];
                int position = 0;
                foreach (byte[] usedBuffer in filled)
                {
                    int remainingBytes = Math.Min(totalBytes - position, 1024);
                    Array.Copy(buffer, 0, data, position, remainingBytes);
                    position += usedBuffer.Length;
                    _buffers.Push(buffer);
                }
                _inbox.Enqueue(new NetworkMessage(data, DeliveryMethod.ReliableOrdered, 0));
            }
        }

        private async void ListenUDP()
        {
            byte[] buffer;
            if (_buffers.Count > 0)
                buffer = _buffers.Pop();
            else
                buffer = new byte[1024];

            SocketReceiveFromResult result = await _udpListener.ReceiveFromAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));
            
            byte[] data = new byte[result.ReceivedBytes];

            Array.Copy(buffer, data, result.ReceivedBytes);

            _buffers.Push(buffer);
            _inbox.Enqueue(new NetworkMessage(data, DeliveryMethod.Unreliable, 0));
        }

        public void Send(INetworkMessage message, INetworkConnection connection)
        {
            if (connection.Status != ConnectionStatus.Connected)
                return;

            switch (message.DeliveryMethod)
            {
                // UDP
                case DeliveryMethod.Unreliable:
                case DeliveryMethod.UnreliableSequenced:
                    SendUDP(message.Data, connection as MNetConnection);
                    break;

                // TCP
                case DeliveryMethod.ReliableUnordered:
                case DeliveryMethod.ReliableSequenced:
                case DeliveryMethod.ReliableOrdered:
                    SendTCP(message.Data, connection as MNetConnection);
                    break;
            }
        }

        private async void SendTCP(byte[] data, MNetConnection connection)
        {
            /*
                If you are using a connection-oriented protocol such as TCP
                Use the Socket, BeginConnect, and EndConnect methods to connect with a listening host. 
                Use the BeginSend and EndSend or BeginReceive and EndReceive methods to communicate data asynchronously. 
                Incoming connection requests can be processed using BeginAccept and EndAccept.
             */

            Socket socket = connection.TCPSocket;
            await socket.ConnectAsync(connection.Endpoint);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, TCPDataSent, connection);
        }

        private void TCPDataSent(IAsyncResult result)
        {
            MNetConnection connection = result.AsyncState as MNetConnection;
            int sentBytes = connection.TCPSocket.EndSend(result);

            connection.TCPSocket.DisconnectAsync(new SocketAsyncEventArgs() { DisconnectReuseSocket = true });
            Log.WriteDebugLine($"[MNet][TCP] Sent {sentBytes} bytes to {connection.Host}");
        }


        private void SendUDP(byte[] data, MNetConnection connection)
        {
            /*
             If you are using a connectionless protocol such as UDP
             Use BeginSendTo and EndSendTo to send datagrams, 
             Use BeginReceiveFrom and EndReceiveFrom to receive datagrams.
            */

            connection.UDPSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, connection.Endpoint, UDPPacketSent, connection);
        }

        private void UDPPacketSent(IAsyncResult result)
        {
            MNetConnection connection = result.AsyncState as MNetConnection;
            int sentBytes = connection.UDPSocket.EndSendTo(result);
            Log.WriteDebugLine($"[MNet][UDP] Sent {sentBytes} bytes to {connection.Host}");
        }
    }
}
