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
        DeviceDX11 _device;

        internal DepthStateBank(DeviceDX11 device)
        {
            _device = device;
        }

        protected override GraphicsDepthState CreatePreset(DepthStencilPreset preset)
        {
            return new GraphicsDepthState(_device, ShaderDepthStencilDefinition.Presets[preset]);
        }
    }
}
