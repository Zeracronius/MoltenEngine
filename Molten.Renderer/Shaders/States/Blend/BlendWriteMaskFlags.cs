using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// Decides which color channels can be written to during a blend operation.
    /// </summary>
    public enum BlendWriteMaskFlags
    {
        /// <summary>
        /// Allow data to be stored in the red component.
        /// </summary>
        Red = 1,

        /// <summary>
        /// Allow data to be stored in the green component.
        /// </summary>
        Green = 2,

        /// <summary>
        /// Allow data to be stored in the blue component.
        /// </summary>
        Blue = 4,

        /// <summary>
        /// Allow data to be stored in the alpha component.
        /// </summary>
        Alpha = 8,

        /// <summary>
        /// Allow data to be stored in all components.
        /// </summary>
        All = 15
    }
}
