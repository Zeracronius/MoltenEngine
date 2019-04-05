using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Molten.Graphics
{
    public class ShaderSampler : PipelineObject<DeviceDX11, PipeDX11>
    {
        SamplerState _state;
        SamplerStateDescription _description;
        bool _isComparison;

        internal ShaderSampler(DeviceDX11 device, ShaderSampler source) : base(device)
        {
            _description = source._description;
        }

        internal ShaderSampler(DeviceDX11 device) : this(device, ShaderSamplerDefinition.Presets[SamplerPreset.Default]) { }

        internal ShaderSampler(DeviceDX11 device, ShaderSamplerDefinition definition) : base(device)
        {
            _description = new SamplerStateDescription()
            {
                AddressU = definition.AddressU.ToApi(),
                AddressV = definition.AddressV.ToApi(),
                AddressW = definition.AddressW.ToApi(),
                BorderColor = definition.BorderColor.ToApi(),
                ComparisonFunction = definition.ComparisonFunc.ToApi(),
                Filter = definition.Filter.ToApi(),
                MaximumAnisotropy = definition.MaxAnisotropy,
                MaximumLod = definition.MaxMipMapLod,
                MinimumLod = definition.MinMipMapLod,
                MipLodBias = definition.LodBias,
            };
        }

        internal override void Refresh(PipeDX11 pipe, PipelineBindSlot<DeviceDX11, PipeDX11> slot)
        {
            base.Refresh(pipe, slot);

            // If the sampler was actually dirty, recreate it.
            if (_state == null)
                _state = new SamplerState(pipe.Device.D3d, _description);
        }

        private protected override void OnPipelineDispose()
        {
            if (_state != null)
                _state.Dispose();
        }

        /// <summary>Gets the underlying sampler state.</summary>
        internal SamplerState State { get { return _state; } }
    }
}
