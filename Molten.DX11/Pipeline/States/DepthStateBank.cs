using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class DepthStateBank : GraphicsStateBank<GraphicsDepthState, DepthStencilPreset>
    {
        internal DepthStateBank(DeviceDX11 device)
        {
            AddPreset(DepthStencilPreset.Default, new GraphicsDepthState(device)
            {
                IsStencilEnabled = true,
            });

            // Default preset
            AddPreset(DepthStencilPreset.DefaultNoStencil, new GraphicsDepthState(device));

            // Z-disabled preset
            AddPreset(DepthStencilPreset.ZDisabled, new GraphicsDepthState(device)
            {
                IsDepthEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero,
            });

            AddPreset(DepthStencilPreset.Sprite2D, new GraphicsDepthState(device)
            {
                IsDepthEnabled = true,
                IsStencilEnabled = true,
                DepthComparison = Comparison.LessEqual,
            });
        }

        internal override GraphicsDepthState GetPreset(DepthStencilPreset value)
        {
            return _presets[(int)value];
        }
    }
}
