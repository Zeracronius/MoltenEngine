using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal class MNetConnectionRequest : Message.ConnectionRequest
    {
        private readonly MNetService _service;
        private readonly MNetConnection _connection;

        public MNetConnectionRequest(byte[] data, DeliveryMethod deliveryMethod, int sequence, MNetConnection connection, MNetService service) 
            : base(data, deliveryMethod, sequence)
        {
            _service = service;
            _connection = connection;
        }

        public override INetworkConnection Connection => _connection;

        public override void Approve()
        {
            _connection.Status = ConnectionStatus.Connected;
            _service.AddConnection(_connection);
        }

        public override void Reject(string reason)
        {
            _service.RejectConnection(_connection);
        }
    }
}
