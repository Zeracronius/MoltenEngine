using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Molten.Net.Message;
using Molten.Threading;

namespace Molten.Net
{
    public class LidgrenNetworkService : NetworkService
    {
        private const int CLIENT_PORT = 3643;

        NetPeerConfiguration _configuration;
        NetPeer _peer;

        protected override ThreadingMode OnStart()
        {
            _configuration = new NetPeerConfiguration(Identity);
            //_configuration.LocalAddress = System.Net.IPAddress.Loopback;

            if (Settings.Network.Mode == NetworkMode.Server)
            {
                _configuration.Port = Settings.Network.Port;
                _configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                _peer = new NetServer(_configuration);
            }
            else
            {
                _configuration.Port = CLIENT_PORT;
                _peer = new NetClient(_configuration);
            }

            _peer.Start();
            Log.WriteLine($"Started network {Settings.Network.Mode} on port {_peer.Port}.");

            return base.OnStart();
        }

        /// <summary>
        /// Gets all connections from underlying network peer.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<INetworkConnection> GetConnections()
        {
            return _peer.Connections.Select(x => new LidgrenConnection(x));
        }

        public override INetworkConnection Connect(string host, int port, byte[] data = null)
        {
            NetConnection connection = null;
            if (data != null)
            {
                NetOutgoingMessage sendMsg = _peer.CreateMessage();
                sendMsg.Write(data);
                connection = _peer.Connect(host, port, sendMsg);
            }
            else
                connection = _peer.Connect(host, port);
            Log.WriteLine($"Connecting to {host}:{port}.");

            return new LidgrenConnection(connection);
        }

        protected override void OnUpdate(Timing timing)
        {
            if (_peer != null)
            {
                ReadMessages();

                if (_peer.Connections.Count > 0)
                    SendMessages();
            }
        }

        private void SendMessages()
        {
            IList<NetConnection> connections = null;
            while (_outbox.TryDequeue(out (INetworkMessage message, INetworkConnection[] recipients) outgoing))
            {
                if (outgoing.recipients == null || outgoing.recipients.Length == 0)
                    connections = _peer.Connections;
                else
                    connections = outgoing.recipients.Select(x => ((LidgrenConnection)x).Connection).ToArray();

                NetOutgoingMessage sendMsg = _peer.CreateMessage();
                sendMsg.Write(outgoing.message.Data);
                _peer.SendMessage(sendMsg, connections, outgoing.message.DeliveryMethod.ToLidgren(), outgoing.message.Sequence);
            }
        }
        private void ReadMessages()
        {
            NetIncomingMessage msg;
            while ((msg = _peer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                        Log.WriteDebugLine(msg.ReadString());
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Log.WriteWarning(msg.ReadString());
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                        Log.WriteError(msg.ReadString());
                        break;

                    case NetIncomingMessageType.ConnectionApproval:
                        _inbox.Enqueue(new LidgrenConnectionRequest(msg));
                        break;

                    case NetIncomingMessageType.Data:
                        byte[] destination = new byte[msg.LengthBytes];
                        Array.Copy(msg.Data, destination, msg.LengthBytes);
                        _inbox.Enqueue(new NetworkMessage(destination, msg.DeliveryMethod.ToMolten(), msg.SequenceChannel));
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        _inbox.Enqueue(new LidgrenConnectionStatusChanged(msg));
                        Log.WriteDebugLine($"Recieved status change: {status} - {msg.ReadString()}");
                        break;
                        
                    default:
                        Log.WriteError("Unhandled message type: " + msg.MessageType);
                        break;
                }
                _peer.Recycle(msg);
            }
        }

        private void Shutdown()
        {
            foreach (NetConnection connection in _peer.Connections)
                connection.Disconnect("Client shutdown.");

            _peer.Shutdown("Client shutdown.");
        }

        protected override void OnStop()
        {
            Shutdown();
            base.OnStop();
        }

        protected override void OnDispose()
        {
            Shutdown();
            base.OnDispose();
        }


    }
}
