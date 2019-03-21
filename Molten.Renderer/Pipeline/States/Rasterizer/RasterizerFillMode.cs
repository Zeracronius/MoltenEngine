using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public enum RasterizerFillMode
    {
        /// <summary>
        /// Draw lines connecting the vertices. Adjacent vertices are not drawn.
        /// </summary>
        Wireframe = 2,

        /// <summary>
        /// Fill the triangles formed by the vertices. Adjacent vertices are not drawn.
        /// </summary>
        Solid = 3
    }
}
