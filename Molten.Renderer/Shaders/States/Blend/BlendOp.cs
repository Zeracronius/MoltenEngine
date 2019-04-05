using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// RGB or alpha blending operation.
    /// </summary>
    public enum BlendOp
    {
        /// <summary>
        /// Add source 1 and source 2.
        /// </summary>
        Add = 1,

        /// <summary>
        ///  Subtract source 1 from source 2.
        /// </summary>
        Subtract = 2,

        /// <summary>
        /// Subtract source 2 from source 1.
        /// </summary>
        ReverseSubtract = 3,

        /// <summary>
        /// Find the minimum of source 1 and source 2.
        /// </summary>
        Minimum = 4,

        /// <summary>
        /// Find the maximum of source 1 and source 2.
        /// </summary>
        Maximum = 5
    }
}
