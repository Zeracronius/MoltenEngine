using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net
{
    public interface INetworkConnection
    {
        ConnectionStatus Status { get; }
        string Host { get; }
        int Port { get; }
    }
}
