using Molten.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet.Tasks
{
    internal abstract class MessageReadTask : WorkerTask
    {
        public MNetRawMessage? Message { get; }
    }
}
