using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class RasterizerStateBank : GraphicsStateBank<GraphicsRasterizerState, RasterizerPreset>
    {
        internal RasterizerStateBank(DeviceDX11 device)
        {
            AddPreset(RasterizerPreset.Default, new GraphicsRasterizerState(device));

            //wireframe preset.
             AddPreset(RasterizerPreset.Wireframe, new GraphicsRasterizerState(device)
            {
                FillMode = FillMode.Wireframe,
            });

            //scissor test preset
             AddPreset(RasterizerPreset.ScissorTest, new GraphicsRasterizerState(device)
            {
                IsScissorEnabled = true,
            });

            //no culling preset.
             AddPreset(RasterizerPreset.NoCulling, new GraphicsRasterizerState(device)
            {
                CullMode = CullMode.None,
            });

             AddPreset(RasterizerPreset.DefaultMultisample, new GraphicsRasterizerState(device)
            {
                IsMultisampleEnabled = true,
            });

             AddPreset(RasterizerPreset.ScissorTestMultisample, new GraphicsRasterizerState(device)
            {
                IsScissorEnabled = true,
                IsMultisampleEnabled = true,
            });
        }

        internal override GraphicsRasterizerState GetPreset(RasterizerPreset value)
        {
            return _presets[(int)value];
        }
    }
}
