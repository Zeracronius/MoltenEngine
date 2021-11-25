using Molten.Networking.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Networking
{
    public interface INetworkConnection
    {
        ConnectionStatus Status { get; }
        string Host { get; }
        int Port { get; }
    }
}
