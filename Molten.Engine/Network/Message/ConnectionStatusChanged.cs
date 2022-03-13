using Molten.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.Message
{
    public abstract class ConnectionStatusChanged : INetworkMessage
    {
        public byte[] Data { get; }
        public int Channel { get; }
        public DeliveryMethod DeliveryMethod { get; }

        protected ConnectionStatusChanged(byte[] data, DeliveryMethod deliveryMethod, int sequence)
        {
            Data = data;
            Channel = sequence;
            DeliveryMethod = deliveryMethod;
        }

        public abstract INetworkConnection Connection { get; }
    }
}
