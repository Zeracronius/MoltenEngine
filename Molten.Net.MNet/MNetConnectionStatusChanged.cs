using Molten.Net.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal class MNetConnectionStatusChanged : ConnectionStatusChanged
    {
        private MNetConnection _connection;

        public MNetConnectionStatusChanged(MNetConnection connection, ConnectionStatus status, byte[] data, DeliveryMethod deliveryMethod, int sequence) 
            : base(data, deliveryMethod, sequence)
        {
            _connection = connection;
            Status = status;
        }

        public override INetworkConnection Connection => _connection;
        public ConnectionStatus Status { get; }
    }
}
