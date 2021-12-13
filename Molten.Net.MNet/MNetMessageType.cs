using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal enum MNetMessageType : byte
    {
        Unknown = 0,
        ConnectionRequest = 1,
        ConnectionApproved = 2,
        ConnectionRejected = 3,
        Data = 4,
        ErrorMessage = 5,
    }
}
