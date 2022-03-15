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
        private readonly List<MNetConnection> _connections;
        private readonly DataWriter _dataWriter;
        private readonly MNetListener _listener;


        public MNetService()
        {
            _connections = new List<MNetConnection>();
            _dataWriter = new DataWriter();
            _listener = new MNetListener();

            OnStarted += MNetService_OnStarted;
        }

        private void MNetService_OnStarted(EngineService o)
        {
            IPAddress localAddress = IPAddress.Parse(Settings.Network.ListeningAddress);
            _listener.Initialize(Thread.Manager, localAddress, Settings.Network.Port);
        }

        public override INetworkConnection Connect(string host, int port, byte[]? data = null)
        {
            MNetConnection connection = new MNetConnection(host, port);
            connection.Status = ConnectionStatus.InitiatedConnect;
            _connections.Add(connection);
            
            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix(_listener.Port, MNetMessageType.ConnectionRequest, DeliveryMethod.ReliableOrdered, 0));
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
            while (_outbox.TryDequeue(out (INetworkMessage, INetworkConnection[]?) message))
                Send(message.Item1, message.Item2?.Cast<MNetConnection>() ?? _connections);

            ProcessRecieved();
        }

        private void ProcessRecieved()
        {
            while (_listener.Inbox.TryDequeue(out MNetRawMessage rawMessage))
            {
                // Process new messages based on transmission method.

                // Identify existing source connection.
                MNetConnection? sourceConnection = _connections.FirstOrDefault(x =>
                {
                    // Compare directly if possible
                    if (x.Endpoint.Address.Equals(rawMessage.Address))
                        return true;

                    // If client connection is IPv6 then compare using that.
                    if (x.Endpoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        return x.Endpoint.Address.Equals(rawMessage.Address.MapToIPv6());

                    // Default to IPv4
                    return x.Endpoint.Address.Equals(rawMessage.Address.MapToIPv4());
                });


                if (sourceConnection != null)
                {
                    sourceConnection.InboundChannels.TryGetValue(rawMessage.Prefix.Channel, out uint currentPacketId);

                    switch (rawMessage.Prefix.DeliveryMethod)
                    {
                        case DeliveryMethod.Unreliable:
                            // Prevent duplicates

                            break;

                        case DeliveryMethod.UnreliableSequenced:
                            // Drop if old
                            if (rawMessage.Prefix.PacketId <= currentPacketId)
                                continue;

                            break;

                        case DeliveryMethod.ReliableUnordered:
                            // Reliable UDP??

                            break;

                        case DeliveryMethod.ReliableSequenced:
                            // Reliable drop if old

                            break;

                        case DeliveryMethod.ReliableOrdered:
                            // Currently TCP

                            break;


                        case DeliveryMethod.Unknown:
                        default:
                            break;
                    }

                    if (currentPacketId < rawMessage.Prefix.PacketId)
                        sourceConnection.InboundChannels[rawMessage.Prefix.Channel] = rawMessage.Prefix.PacketId;
                }

                INetworkMessage message;
                switch (rawMessage.Prefix.Type)
                {
                    case MNetMessageType.ConnectionRequest:
                        MNetConnection newConnection = new MNetConnection(rawMessage.Address, rawMessage.Prefix.Channel);
                        message = new MNetConnectionRequest(rawMessage.Data, rawMessage.Prefix.DeliveryMethod, 0, newConnection, this);
                        break;

                    case MNetMessageType.ConnectionApproved:
                        if (sourceConnection == null)
                            throw new InvalidOperationException("Approved a non-existing connection.");

                        Log.Write("[Network] Connection approved: " + sourceConnection.ToString());
                        sourceConnection.Status = ConnectionStatus.Connected;

                        message = new MNetConnectionStatusChanged(sourceConnection, ConnectionStatus.Connected, rawMessage.Data, rawMessage.Prefix.DeliveryMethod, 0);
                        break;

                    case MNetMessageType.ConnectionRejected:
                        if (sourceConnection == null)
                            throw new InvalidOperationException("Rejected a non-existing connection.");

                        Log.Write("[Network] Connection rejected: " + sourceConnection.ToString());
                        sourceConnection.Status = ConnectionStatus.Disconnected;
                        _connections.Remove(sourceConnection);

                        message = new MNetConnectionStatusChanged(sourceConnection, ConnectionStatus.Disconnected, rawMessage.Data, rawMessage.Prefix.DeliveryMethod, 0);
                        break;

                    case MNetMessageType.Data:
                        message = new NetworkMessage(rawMessage.Data, rawMessage.Prefix.PacketId, rawMessage.Prefix.DeliveryMethod, 0, sourceConnection);
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


        public void Send(INetworkMessage message, IEnumerable<MNetConnection> connections)
        {
            foreach (MNetConnection connection in connections)
            {
                if (connection.Status != ConnectionStatus.Connected)
                    return;

                _dataWriter.Clear();
                _dataWriter.Write(new MessagePrefix(message.Channel, MNetMessageType.Data, message.DeliveryMethod, connection.GetOutboundPacketId(message.Channel)));
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
            MNetConnection connection = (MNetConnection)result.AsyncState!;
            int sentBytes = connection.TCPSocket.EndSend(result);

            var e = new SocketAsyncEventArgs()
            {
                DisconnectReuseSocket = true,
                UserToken = connection,
            };
            e.Completed += TCPConnectionDisconnected;
            connection.TCPSocket.DisconnectAsync(e);
            Log.Debug($"[MNet][TCP] Sent {sentBytes} bytes to {connection.Host}");
        }

        private void TCPConnectionDisconnected(object? sender, SocketAsyncEventArgs e)
        {
            ((MNetConnection)e.UserToken!).TCPWaitHandle.Set();
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
            MNetConnection connection = (MNetConnection)result.AsyncState!;
            int sentBytes = connection.UDPSocket.EndSendTo(result);
            Log.Debug($"[MNet][UDP] Sent {sentBytes} bytes to {connection.Host}");
            connection.UDPWaitHandle.Set();
        }


        internal void AddConnection(MNetConnection connection)
        {
            _connections.Add(connection);

            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix(0, MNetMessageType.ConnectionApproved, DeliveryMethod.ReliableOrdered, 0));
            SendTCP(_dataWriter.GetData(), connection);
        }

        internal void RejectConnection(MNetConnection connection, string reason)
        {
            _dataWriter.Clear();
            _dataWriter.Write(new MessagePrefix(0, MNetMessageType.ConnectionRejected, DeliveryMethod.ReliableOrdered, 0));
            if (reason != null)
                _dataWriter.WriteString(reason, Encoding.UTF8);

            SendTCP(_dataWriter.GetData(), connection);
        }

        protected override void OnDispose()
        {
            _listener.Dispose();
            base.OnDispose();
        }
    }
}
