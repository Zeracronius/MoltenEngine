using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>Stores a rasterizer state for use with a <see cref="PipeDX11"/>.</summary>
    internal class GraphicsRasterizerState : PipelineObject<DeviceDX11, PipeDX11>
    {
        internal RasterizerState State;
        RasterizerStateDescription _desc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">An existing <see cref="GraphicsRasterizerState"/> instance from which to copy settings."/></param>
        internal GraphicsRasterizerState(DeviceDX11 device, GraphicsRasterizerState source) : base(device)
        {
            _desc = source._desc;
        }

        internal GraphicsRasterizerState(DeviceDX11 device) : this(device, ShaderRasterizerDefinition.Presets[RasterizerPreset.Default]) { }

        internal GraphicsRasterizerState(DeviceDX11 device, ShaderRasterizerDefinition definition) : base(device)
        {
            _desc = new RasterizerStateDescription()
            {
                CullMode = (CullMode)definition.CullMode,
                DepthBias = definition.DepthBias,
                DepthBiasClamp = definition.DepthBiasClamp,
                FillMode = (FillMode)definition.FillMode,
                IsAntialiasedLineEnabled = definition.IsAntialiasedLineEnabled,
                IsDepthClipEnabled = definition.IsDepthClipEnabled,
                IsFrontCounterClockwise = definition.IsFrontCounterClockwise,
                IsMultisampleEnabled = definition.IsMultisampleEnabled,
                IsScissorEnabled = definition.IsScissorEnabled,
                SlopeScaledDepthBias = definition.SlopeScaledDepthBias,
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphicsRasterizerState other)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(GraphicsRasterizerState other)
        {
            return _desc.CullMode == other._desc.CullMode &&
                _desc.DepthBias == other._desc.DepthBias &&
                _desc.DepthBiasClamp == other._desc.DepthBiasClamp &&
                _desc.FillMode == other._desc.FillMode &&
                _desc.IsAntialiasedLineEnabled == other._desc.IsAntialiasedLineEnabled &&
                _desc.IsDepthClipEnabled == other._desc.IsDepthClipEnabled &&
                _desc.IsFrontCounterClockwise == other._desc.IsFrontCounterClockwise &&
                _desc.IsMultisampleEnabled == other._desc.IsMultisampleEnabled &&
                _desc.IsScissorEnabled == other._desc.IsScissorEnabled &&
                _desc.SlopeScaledDepthBias == other._desc.SlopeScaledDepthBias;
        }

        internal override void Refresh(PipeDX11 pipe, PipelineBindSlot<DeviceDX11, PipeDX11> slot)
        {
            if (State == null)
                State = new RasterizerState(pipe.Device.D3d, _desc);
        }

        private protected override void OnPipelineDispose()
        {
            DisposeObject(ref State);
        }
    }
}
