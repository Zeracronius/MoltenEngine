using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.Message
{
    public struct NetworkMessage : INetworkMessage
    {
        public byte[] Data { get; }
        public int Channel { get; }
        public DeliveryMethod DeliveryMethod { get; }
        public ulong PacketId { get; }
        public INetworkConnection? Connection { get; }

        public NetworkMessage(byte[] data, ulong packetId, DeliveryMethod deliveryMethod, int channel, INetworkConnection? connection)
        {
            Data = data;
            Channel = channel;
            PacketId = packetId;
            DeliveryMethod = deliveryMethod;
            Connection = connection;
        }


        public NetworkMessage(byte[] data, DeliveryMethod deliveryMethod, int channel)
        {
            Data = data;
            Channel = channel;
            PacketId = 0;
            DeliveryMethod = deliveryMethod;
            Connection = null;
        }
    }
}
