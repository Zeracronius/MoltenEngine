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
        DeviceDX11 _device;

        internal RasterizerStateBank(DeviceDX11 device)
        {
            _device = device;
        }

        protected override GraphicsRasterizerState CreatePreset(RasterizerPreset preset)
        {
            return new GraphicsRasterizerState(_device, ShaderRasterizerDefinition.Presets[preset]);
        }
    }
}
