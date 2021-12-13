﻿using Molten.Collections;
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

        Thread _tcpListeningThread;
        Thread _udpListeningThread;

        private struct MessagePrefix
        {
            internal int Sequence { get; }
            internal MNetMessageType Type { get; }

            public MessagePrefix(int sequence, MNetMessageType type)
            {
                Sequence = sequence;
                Type = type;
            }
        }
        private static int MessagePrefixSize = System.Runtime.InteropServices.Marshal.SizeOf<MessagePrefix>();

        public MNetService()
        {
            _connections = new List<MNetConnection>();

            _tcpListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _udpListener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _buffers = new ThreadedQueue<byte[]>();
        }

        protected override void OnInitialize(EngineSettings settings, Logger log)
        {
            base.OnInitialize(settings, log);

            IPAddress localAddress = new IPAddress(this.Settings.Network.ListeningAddress.Value.Split('.').Select(x => byte.Parse(x)).ToArray());
            _localEndPoint = new IPEndPoint(localAddress, this.Settings.Network.Port);

            _udpListener.Bind(new IPEndPoint(IPAddress.Any, Settings.Network.Port));
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

            DataWriter writer = new DataWriter();
            writer.Write(new MessagePrefix(0, MNetMessageType.ConnectionRequest));
            if (data != null)
                writer.Write(data);

            SendTCP(writer.GetData(), connection);
            return connection;
        }

        public override IEnumerable<INetworkConnection> GetConnections()
        {
            return _connections;
        }

        protected override void OnUpdate(Timing time)
        {
            // Handle incoming.

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

                    INetworkMessage message;
                    switch (messagePrefix.Type)
                    {
                        case MNetMessageType.ConnectionRequest:
                            IPEndPoint remoteIP = connection.RemoteEndPoint as IPEndPoint;
                            MNetConnection newConnection = new MNetConnection(remoteIP);
                            message = new MNetConnectionRequest(data, DeliveryMethod.ReliableOrdered, messagePrefix.Sequence, newConnection, this);
                            break;

                        //case MNetMessageType.ConnectionApproved:
                        //    break;

                        //case MNetMessageType.ConnectionRejected:
                        //    break;

                        case MNetMessageType.Data:
                            message = new NetworkMessage(data, DeliveryMethod.ReliableOrdered, 0);
                            break;

                        //case MNetMessageType.ErrorMessage:
                        //    break;
                
                        case MNetMessageType.Unknown:
                        default:
                            throw new NotImplementedException();
                    }


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

                int receivedBytes = _udpListener.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _localEndPoint);


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
                        message = new NetworkMessage(data, DeliveryMethod.Unreliable, messagePrefix.Sequence);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                _inbox.Enqueue(message);
            }
        }

        public void Send(INetworkMessage message, IEnumerable<MNetConnection> connections)
        {
            DataWriter writer = new DataWriter();
            writer.Write(new MessagePrefix(message.Sequence, MNetMessageType.Data));
            writer.Write(message.Data);

            switch (message.DeliveryMethod)
            {
                // UDP
                case DeliveryMethod.Unreliable:
                case DeliveryMethod.UnreliableSequenced:
                    SendUDP(message.Data, connections.Cast<MNetConnection>());
                    break;

                // TCP
                case DeliveryMethod.ReliableUnordered:
                case DeliveryMethod.ReliableSequenced:
                case DeliveryMethod.ReliableOrdered:
                    foreach (MNetConnection connection in connections)
                    {
                        // Yield until socket is available.
                        while (connection.TCPWaitHandle.WaitOne(10) == false)
                            System.Threading.Thread.Yield();
                        
                        SendTCP(message.Data, connection);
                    }

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

            var e = new SocketAsyncEventArgs()
            {
                DisconnectReuseSocket = true,
            };

            connection.TCPSocket.DisconnectAsync(e);
            Log.WriteDebugLine($"[MNet][TCP] Sent {sentBytes} bytes to {connection.Host}");
        }


        private void SendUDP(byte[] data, IEnumerable<MNetConnection> connections)
        {
            /*
             If you are using a connectionless protocol such as UDP
             Use BeginSendTo and EndSendTo to send datagrams, 
             Use BeginReceiveFrom and EndReceiveFrom to receive datagrams.
            */

            foreach (MNetConnection connection in connections)
            {
                // Yield until socket is available.
                while (connection.TCPWaitHandle.WaitOne(10) == false)
                    System.Threading.Thread.Yield();

                connection.UDPSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, connection.Endpoint, UDPPacketSent, connection);
            }
        }

        private void UDPPacketSent(IAsyncResult result)
        {
            MNetConnection connection = result.AsyncState as MNetConnection;
            int sentBytes = connection.UDPSocket.EndSendTo(result);
            Log.WriteDebugLine($"[MNet][UDP] Sent {sentBytes} bytes to {connection.Host}");
        }


        internal void AddConnection(MNetConnection connection)
        {
            _connections.Add(connection);
            //TODO Send Approval
        }

        internal void RejectConnection(MNetConnection connection)
        {
            //TODO Send Rejection
        }

        protected override void OnDispose()
        {
            _tcpListeningThread.Abort();
            _udpListeningThread.Abort();

            base.OnDispose();
        }
    }
}
