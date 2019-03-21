using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class BlendStateBank : GraphicsStateBank<GraphicsBlendState, BlendStatePreset>
    {
        DeviceDX11 _device;

        internal BlendStateBank(DeviceDX11 device)
        {
            _device = device;
        }

        protected override GraphicsBlendState CreatePreset(BlendStatePreset preset)
        {
            return new GraphicsBlendState(_device, ShaderBlendStateDefinition.Presets[preset]);
        }
    }
}
