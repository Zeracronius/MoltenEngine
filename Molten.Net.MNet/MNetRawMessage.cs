using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal struct MNetRawMessage
    {
        public MessagePrefix Prefix { get; }
        public byte[] Data { get; }
        public System.Net.IPAddress Address { get; }
        public DateTime RecievedDate { get; }

        public MNetRawMessage(MessagePrefix prefix, byte[] data, System.Net.IPAddress address)
        {
            Prefix = prefix;
            Data = data;
            Address = address;
            RecievedDate = DateTime.Now;
        }
    }
}
