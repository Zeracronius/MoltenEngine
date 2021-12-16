using Molten.Font;
using Molten.Graphics;
using Molten.Net;
using Molten.Net.Message;
using Molten.Net.MNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Samples
{
    public class NetworkingTest : NetSampleGame<MNetService>
    {
        public override string Description => "A basic networking test.";

        Net.MNet.MNetService _client;
        Threading.ThreadManager _clientThreadManager;
        INetworkConnection _serverConnection;

        public NetworkingTest() 
            : base("Networking Test")
        {

        }

        protected override void OnStart(EngineSettings settings)
        {
            base.OnStart(settings);
            settings.Network.Mode = NetworkMode.Server;
            settings.Network.ListeningAddress.Value = "127.0.0.1";
        }

        protected override void OnInitialize(Engine engine)
        {
            base.OnInitialize(engine);

            EngineSettings clientSettings = new EngineSettings();
            clientSettings.Network.Mode = NetworkMode.Client;
            clientSettings.Network.Port.Value = 6114;
            clientSettings.Network.ListeningAddress.Value = "127.0.0.1";

            _clientThreadManager = new Threading.ThreadManager(Log);
            _client = new MNetService();
            _client.Identity = Net.Identity;
            _client.Initialize(clientSettings, Log);
            _client.Start(_clientThreadManager, Log);
            _serverConnection = _client.Connect(System.Net.IPAddress.Loopback.ToString(), Settings.Network.Port, Encoding.UTF8.GetBytes("Hail!"));
        }

        protected override void OnUpdate(Timing time)
        {
            if (_serverConnection.Status == ConnectionStatus.Connected)
            {
                DataWriter writer = new DataWriter();
                writer.Write<byte>(1);
                writer.Write(1);
                writer.WriteString("Message" + time.CurrentFrame, Encoding.UTF8);
                writer.WriteStringRaw("In other news...", Encoding.UTF8);
                _client.SendMessage(new NetworkMessage(writer.GetData(), DeliveryMethod.Unreliable, 0));
            }


            while (Engine.Net.TryReadMessage(out var iMessage))
            {
                switch (iMessage)
                {
                    case ConnectionRequest connectionRequest:
                        string hailMessage = Encoding.UTF8.GetString(connectionRequest.Data);
                        connectionRequest.Approve();
                        Log.WriteDebugLine("[Server]: Approved connection request: " + hailMessage);
                        break;

                    case NetworkMessage message:


                        DataReader reader = new DataReader(message.Data);
                        reader.Read<byte>();
                        reader.Read<int>();
                        string messageContent = reader.ReadString(Encoding.UTF8);
                        string anotherString = reader.ReadString(Encoding.UTF8);


                        //string messageContent = Encoding.ASCII.GetString(message.Data, 1, message.Data.Length - 1);
                        Log.WriteDebugLine("[Server]: Recieved message: " + messageContent);
                        break;

                    case ConnectionStatusChanged message:
                        string content = Encoding.ASCII.GetString(message.Data);
                        Log.WriteDebugLine($"[Server][{message.Connection.Host}]: Connection status changed: " + content);
                        break;

                    default:
                        break;
                }
            }

            UpdateClient(time);
            base.OnUpdate(time);

            if (base.RunState == GameRunState.Exiting)
                _client.Dispose();
        }

        private void UpdateClient(Timing time)
        {
            _client.Update(time);


            while (_client.TryReadMessage(out var iMessage))
            {
                switch (iMessage)
                {
                    //case ConnectionRequest connectionRequest:
                    //    string hailMessage = Encoding.ASCII.GetString(connectionRequest.Data);
                    //    Console.WriteLine("[Client] Recieved connection request: " + hailMessage);
                    //    connectionRequest.Approve();
                    //    break;

                    case NetworkMessage message:

                        DataReader reader = new DataReader(message.Data);
                        reader.Read<byte>();
                        string messageContent = reader.ReadString(Encoding.UTF8);
                        string anotherString = reader.ReadString(Encoding.UTF8);


                        //string messageContent = Encoding.ASCII.GetString(message.Data, 1, message.Data.Length - 1);
                        Log.WriteDebugLine("[Client]: Recieved message: " + messageContent);
                        break;

                    case ConnectionStatusChanged message:
                        string content = Encoding.ASCII.GetString(message.Data);
                        Log.WriteDebugLine($"[Client][{message.Connection.Host}]: Connection status changed: " + content);
                        break;

                    default:
                        break;
                }
            }
        }

        protected override void OnClose()
        {
            _client.Dispose();
            _clientThreadManager.Dispose();
        }
    }
}
