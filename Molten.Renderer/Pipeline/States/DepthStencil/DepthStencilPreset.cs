using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public enum DepthStencilPreset
    {
        /// <summary>Default depth stencil state with stencil testing enabled.</summary>
        Default = 0,

        /// <summary>The default depth stencil state, but with stencil testing disabled.</summary>
        DefaultNoStencil = 1,

        /// <summary>The same as default, but with the z-buffer disabled.</summary>
        ZDisabled = 2,

        /// <summary>A state used for drawing 2D sprites. Stenicl testing is enabled.</summary>
        Sprite2D = 3,
    }
}
