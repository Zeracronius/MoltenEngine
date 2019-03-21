using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public enum StencilOp
    {
        /// <summary>
        /// Keep the existing stencil data.
        /// </summary>
        Keep = 1,

        /// <summary>
        /// Set the stencil data to 0.
        /// </summary>
        Zero = 2,

        /// <summary>
        /// Set the stencil data to the reference value set by calling ID3D11DeviceContext::OMSetDepthStencilState.
        /// </summary>
        Replace = 3,

        /// <summary>
        /// Increment the stencil value by 1, and clamp the result.
        /// </summary>
        IncrementAndClamp = 4,

        /// <summary>
        /// Decrement the stencil value by 1, and clamp the result.
        /// </summary>
        DecrementAndClamp = 5,

        /// <summary>
        /// Invert the stencil data.
        /// </summary>
        Invert = 6,

        /// <summary>
        /// Increment the stencil value by 1, and wrap the result if necessary.
        /// </summary>
        Increment = 7,

        /// <summary>
        ///  Decrement the stencil value by 1, and wrap the result if necessary.
        /// </summary>
        Decrement = 8
    }
}
