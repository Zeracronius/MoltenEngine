using Molten.Collections;
using Molten.Net.Message;
using Molten.Threading;
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
    public class MNetService : NetworkService
    {
        private List<MNetConnection> _connections;
        private Socket _tcpListener;
        private Socket _udpListener;
        private ThreadedQueue<byte[]> _buffers;
        private EndPoint _localEndPoint;

        private Thread _tcpListeningThread;
        private Thread _udpListeningThread;

        private DataWriter _dataWriter;

        private struct MessagePrefix
        {
            internal int Channel { get; }
            internal MNetMessageType Type { get; }
            internal uint SequenceNumber { get; }

            public MessagePrefix(int channel, MNetMessageType type, uint sequenceNumber)
            {
                Channel = channel;
                Type = type;
                SequenceNumber = sequenceNumber;
            }
        }
        private static int MessagePrefixSize = System.Runtime.InteropServices.Marshal.SizeOf<MessagePrefix>();

        public MNetService()
        {
            _connections = new List<MNetConnection>();

            _tcpListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _udpListener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _buffers = new ThreadedQueue<byte[]>();
            _dataWriter = new DataWriter();
        }

        protected override void OnInitialize(EngineSettings settings, Logger log)
        {
            base.OnInitialize(settings, log);

            IPAddress localAddress = new IPAddress(settings.Network.ListeningAddress.Value.Split('.').Select(x => byte.Parse(x)).ToArray());
            _localEndPoint = new IPEndPoint(localAddress, settings.Network.Port.Value);

            _udpListener.Bind(new IPEndPoint(IPAddress.Any, settings.Network.Port.Value));
            _tcpListener.Bind(_localEndPoint);
            _tcpListener.Listen(100);

            _udpListeningThread = new Thread(ListenUDP);
            _udpListeningThread.Start();

            _tcpListeningThread = new Thread(ListenTCP);
            _tcpListeningThread.Start();
        }

        public override INetworkConnection Connect(string host, int port, byte[] data = null)
        {
            MNetConnection connection = new MNetConnection(host, port);
            connection.Status = ConnectionStatus.InitiatedConnect;
            _connections.Add(connection);

            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix((_localEndPoint as IPEndPoint).Port, MNetMessageType.ConnectionRequest, 0));
            if (data != null)
                _dataWriter.Write(data);

            SendTCP(_dataWriter.GetData(), connection);
            return connection;
        }

        public override IEnumerable<INetworkConnection> GetConnections()
        {
            return _connections;
        }

        protected override void OnUpdate(Timing time)
        {
            // Handle outgoing.
            while (_outbox.TryDequeue(out (INetworkMessage, INetworkConnection[]) message))
                Send(message.Item1, message.Item2?.Cast<MNetConnection>() ?? _connections);
        }

        private void ListenTCP()
        {
            while(true)
            {
                using (Socket connection = _tcpListener.Accept())
                {
                    EndPoint remoteEndpoint = connection.RemoteEndPoint;

                    Queue<byte[]> filled = new Queue<byte[]>();
                    int bytesRecieved = 0;
                    int totalBytes = 0;
                    byte[] buffer;
                    do
                    {
                        if (_buffers.TryDequeue(out buffer) == false)
                            buffer = new byte[1024];

                        bytesRecieved = connection.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        if (bytesRecieved == 0)
                            break;

                        totalBytes += bytesRecieved;
                        filled.Enqueue(buffer);

                        if (bytesRecieved < buffer.Length)
                            break;
                    }
                    while (buffer[bytesRecieved] != 0x1A);

                    connection.Disconnect(true);

                    int prefix = MessagePrefixSize;
                    byte[] data = new byte[totalBytes - prefix];
                    int position = 0;
                    MessagePrefix messagePrefix = new MessagePrefix();
                    while (filled.Count > 0)
                    {
                        byte[] usedBuffer = filled.Dequeue();
                        int remainingBytes = Math.Min(totalBytes - prefix - position, 1024);

                        Array.Copy(buffer, prefix, data, position, remainingBytes);
                        position += usedBuffer.Length;

                        // Read prefix
                        if (prefix > 0)
                        {
                            unsafe
                            {
                                MessagePrefix* prefixPointer = &messagePrefix;
                                fixed (byte* bufferPointer = buffer)
                                    Buffer.MemoryCopy(bufferPointer, prefixPointer, MessagePrefixSize, MessagePrefixSize);
                            }
                            prefix = 0;
                        }

                        _buffers.Enqueue(buffer);
                    }

                    INetworkMessage message = null;
                    switch (messagePrefix.Type)
                    {
                        case MNetMessageType.ConnectionRequest:
                            IPEndPoint remoteIP = connection.RemoteEndPoint as IPEndPoint;
                            MNetConnection newConnection = new MNetConnection(remoteIP.Address.MapToIPv4(), messagePrefix.Channel);
                            message = new MNetConnectionRequest(data, DeliveryMethod.ReliableOrdered, 0, newConnection, this);
                            break;

                        case MNetMessageType.ConnectionApproved:
                            Log.Write("Connection approved.");
                            MNetConnection mnetConnectionApproved = _connections.FirstOrDefault(x => x.Endpoint.Address.Equals((connection.RemoteEndPoint as IPEndPoint).Address.MapToIPv4()));
                            if (mnetConnectionApproved != null)
                                mnetConnectionApproved.Status = ConnectionStatus.Connected;

                            message = new MNetConnectionStatusChanged(mnetConnectionApproved, ConnectionStatus.Connected, data, DeliveryMethod.ReliableOrdered, 0);
                            break;

                        case MNetMessageType.ConnectionRejected:
                            Log.Write("Connection rejected: " + Encoding.UTF8.GetString(data));
                            MNetConnection mnetConnectionRejected = _connections.FirstOrDefault(x => x.Endpoint.Address.Equals((connection.RemoteEndPoint as IPEndPoint).Address.MapToIPv4()));
                            if (mnetConnectionRejected != null)
                                mnetConnectionRejected.Status = ConnectionStatus.Disconnected;
                            _connections.Remove(mnetConnectionRejected);

                            message = new MNetConnectionStatusChanged(mnetConnectionRejected, ConnectionStatus.Disconnected, data, DeliveryMethod.ReliableOrdered, 0);
                            break;

                        case MNetMessageType.Data:
                            message = new NetworkMessage(data, DeliveryMethod.ReliableOrdered, 0);
                            break;

                        //case MNetMessageType.ErrorMessage:
                        //    break;
                
                        case MNetMessageType.Unknown:
                        default:
                            throw new NotImplementedException();
                    }

                    if (message != null)
                        _inbox.Enqueue(message);
                }
            }
        }

        private void ListenUDP()
        {
            while (true)
            {
                byte[] buffer;
                if (_buffers.TryDequeue(out buffer) == false)
                    buffer = new byte[1024];

                EndPoint endpoint = new IPEndPoint(IPAddress.Any, (_localEndPoint as IPEndPoint).Port);
                int receivedBytes = _udpListener.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint);


                byte[] data = new byte[receivedBytes - MessagePrefixSize];

                // Read prefix
                MessagePrefix messagePrefix;
                unsafe
                {
                    MessagePrefix* prefixPointer = &messagePrefix;
                    fixed (byte* bufferPointer = buffer)
                        Buffer.MemoryCopy(bufferPointer, prefixPointer, MessagePrefixSize, MessagePrefixSize);
                }
                Array.Copy(buffer, MessagePrefixSize, data, 0, data.Length);

                _buffers.Enqueue(buffer);

                INetworkMessage message;
                switch (messagePrefix.Type)
                {
                    case MNetMessageType.Data:
                        message = new NetworkMessage(data, DeliveryMethod.Unreliable, messagePrefix.Channel);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                _inbox.Enqueue(message);
            }
        }

        public void Send(INetworkMessage message, IEnumerable<MNetConnection> connections)
        {
            foreach (MNetConnection connection in connections)
            {
                if (connection.Status != ConnectionStatus.Connected)
                    return;

                _dataWriter.Clear();
                _dataWriter.Write(new MessagePrefix(message.Channel, MNetMessageType.Data, connection.GetSequenceNumber()));
                _dataWriter.Write(message.Data);

                switch (message.DeliveryMethod)
                {
                    // UDP
                    case DeliveryMethod.Unreliable:
                    case DeliveryMethod.UnreliableSequenced:
                        SendUDP(_dataWriter.GetData(), connection);
                    break;

                    // TCP
                    case DeliveryMethod.ReliableUnordered:
                    case DeliveryMethod.ReliableSequenced:
                    case DeliveryMethod.ReliableOrdered:
                        SendTCP(_dataWriter.GetData(), connection);
                    break;
                }
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

            // Yield until socket is available.
            while (connection.TCPWaitHandle.WaitOne(10) == false)
                System.Threading.Thread.Yield();

            connection.TCPWaitHandle.Reset();
            Socket socket = connection.TCPSocket;
            await socket.ConnectAsync(connection.Endpoint);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, TCPDataSent, connection);
        }

        private void TCPDataSent(IAsyncResult result)
        {
            MNetConnection connection = result.AsyncState as MNetConnection;
            int sentBytes = connection.TCPSocket.EndSend(result);

            var e = new SocketAsyncEventArgs()
            {
                DisconnectReuseSocket = true,
                UserToken = connection,
            };
            e.Completed += TCPConnectionDisconnected;
            connection.TCPSocket.DisconnectAsync(e);
            Log.WriteDebugLine($"[MNet][TCP] Sent {sentBytes} bytes to {connection.Host}");
        }

        private void TCPConnectionDisconnected(object sender, SocketAsyncEventArgs e)
        {
            (e.UserToken as MNetConnection).TCPWaitHandle.Set();
        }

        private void SendUDP(byte[] data, MNetConnection connection)
        {
            /*
             If you are using a connectionless protocol such as UDP
             Use BeginSendTo and EndSendTo to send datagrams, 
             Use BeginReceiveFrom and EndReceiveFrom to receive datagrams.
            */

            // Yield until socket is available.
            while (connection.UDPWaitHandle.WaitOne(10) == false)
                System.Threading.Thread.Yield();

            connection.UDPWaitHandle.Reset();
            connection.UDPSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, connection.Endpoint, UDPPacketSent, connection);
        }

        private void UDPPacketSent(IAsyncResult result)
        {
            MNetConnection connection = result.AsyncState as MNetConnection;
            int sentBytes = connection.UDPSocket.EndSendTo(result);
            Log.WriteDebugLine($"[MNet][UDP] Sent {sentBytes} bytes to {connection.Host}");
            connection.UDPWaitHandle.Set();
        }


        internal void AddConnection(MNetConnection connection)
        {
            _connections.Add(connection);

            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix(0, MNetMessageType.ConnectionApproved, 0));
            SendTCP(_dataWriter.GetData(), connection);
        }

        internal void RejectConnection(MNetConnection connection, string reason)
        {
            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix(0, MNetMessageType.ConnectionRejected, 0));
            if (reason != null)
                _dataWriter.WriteString(reason, Encoding.UTF8);

            SendTCP(_dataWriter.GetData(), connection);
        }

        protected override void OnDispose()
        {
            _tcpListeningThread.Abort();
            _udpListeningThread.Abort();

            base.OnDispose();
        }
    }
}
