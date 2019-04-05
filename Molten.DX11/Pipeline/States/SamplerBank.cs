using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class SamplerBank : GraphicsStateBank<ShaderSampler, SamplerPreset>
    {
        DeviceDX11 _device;

        internal SamplerBank(DeviceDX11 device)
        {
            _device = device;
        }

        protected override ShaderSampler CreatePreset(SamplerPreset preset)
        {
            return new ShaderSampler(_device, ShaderSamplerDefinition.Presets[preset]);
        }
    }

}
