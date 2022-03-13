using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net
{
    public enum DeliveryMethod : byte
    {
        Unknown = 0,

        /// <summary>
        /// No guarantees, except for preventing duplicates.
        /// </summary>
        Unreliable = 1,

        /// <summary>
        /// Late messages will be dropped if newer ones were already received.
        /// </summary>
        UnreliableSequenced = 2,

        /// <summary>
        /// All packages will arrive, but not necessarily in the same order.
        /// </summary>
        ReliableUnordered = 3,

        /// <summary>
        /// All packages will arrive, but late ones will be dropped.
        /// </summary>
        ReliableSequenced = 4,

        /// <summary>
        /// This means that we will always receive the latest message eventually, but may miss older ones.
        /// Unlike all the other methods, here the library will hold back messages until all previous ones are received, before handing them to us.
        /// All packages will arrive, and they will do so in the same order.
        /// </summary>
        ReliableOrdered = 5,
    }
}
