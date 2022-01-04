using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal struct MessagePrefix
    {
        internal static int Size = System.Runtime.InteropServices.Marshal.SizeOf<MessagePrefix>();

        internal int Channel { get; }
        internal MNetMessageType Type { get; }
        internal uint PacketId { get; }
        internal DeliveryMethod DeliveryMethod { get; }

        public MessagePrefix(int channel, MNetMessageType type, DeliveryMethod deliveryMethod, uint packetId)
        {
            Channel = channel;
            Type = type;
            PacketId = packetId;
            DeliveryMethod = deliveryMethod;
        }
    }
}
