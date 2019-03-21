using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public enum RasterizerCullMode
    {
        /// <summary>
        /// Always draw all triangles.
        /// </summary>
        None = 1,

        /// <summary>
        /// Do not draw triangles that are front-facing.
        /// </summary>
        Front = 2,

        /// <summary>
        /// Do not draw triangles that are back-facing.
        /// </summary>
        Back = 3
    }
}
