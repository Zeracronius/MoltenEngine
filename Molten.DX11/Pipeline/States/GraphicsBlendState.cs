using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>Stores a blend state for use with a <see cref="PipeDX11"/>.</summary>
    internal class GraphicsBlendState : PipelineObject<DeviceDX11, PipeDX11>, IEquatable<GraphicsBlendState>
    {
        internal BlendState State;
        BlendStateDescription _desc;

        internal GraphicsBlendState(DeviceDX11 device, GraphicsBlendState source) : base(device)
        {
            _desc = source._desc.Clone();
            BlendFactor = source.BlendFactor;
            BlendSampleMask = source.BlendSampleMask;
        }

        internal GraphicsBlendState(DeviceDX11 device) : this(device, ShaderBlendStateDefinition.Presets[BlendStatePreset.Default])
        {
            _desc = BlendStateDescription.Default();
        }

        internal GraphicsBlendState(DeviceDX11 device, ShaderBlendStateDefinition definition) : base(device)
        {
            _desc = new BlendStateDescription()
            {
                AlphaToCoverageEnable = definition.AlphaToCoverageEnable,
                IndependentBlendEnable = definition.IndependentBlendEnable,
            };

            ShaderBlendSlotDefinition slotDef = definition.Targets[0];

            _desc.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                AlphaBlendOperation = (BlendOperation)slotDef.AlphaBlendOperation,
                BlendOperation = (BlendOperation)slotDef.BlendOperation,
                DestinationAlphaBlend = (BlendOption)slotDef.DestinationAlphaBlend,
                DestinationBlend = (BlendOption)slotDef.DestinationBlend,
                IsBlendEnabled = slotDef.IsBlendEnabled,
                RenderTargetWriteMask = (ColorWriteMaskFlags)slotDef.RenderTargetWriteMask,
                SourceAlphaBlend = (BlendOption)slotDef.SourceAlphaBlend,
                SourceBlend = (BlendOption)slotDef.SourceBlend,
            };
            BlendFactor = definition.BlendFactor;
            BlendSampleMask = definition.BlendSampleMask;
        }

        internal RenderTargetBlendDescription GetSurfaceBlendState(int index)
        {
            return _desc.RenderTarget[index];
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphicsBlendState other)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(GraphicsBlendState other)
        {
            if (_desc.IndependentBlendEnable != other.IndependentBlendEnable)
                return false;

            if (_desc.AlphaToCoverageEnable != other.AlphaToCoverageEnable)
                return false;

            // Equality check against all RT blend states
            for(int i = 0; i < _desc.RenderTarget.Length; i++)
            {
                RenderTargetBlendDescription rt = _desc.RenderTarget[i];
                RenderTargetBlendDescription otherRt = other._desc.RenderTarget[i];

                if (rt.AlphaBlendOperation != otherRt.AlphaBlendOperation ||
                    rt.BlendOperation != otherRt.BlendOperation ||
                    rt.DestinationAlphaBlend != otherRt.DestinationAlphaBlend ||
                    rt.DestinationBlend != otherRt.DestinationBlend ||
                    rt.IsBlendEnabled != otherRt.IsBlendEnabled ||
                    rt.RenderTargetWriteMask != otherRt.RenderTargetWriteMask ||
                    rt.SourceAlphaBlend != otherRt.SourceAlphaBlend ||
                    rt.SourceBlend != otherRt.SourceBlend)
                {
                    return false;
                }
            }
            return true;
        }

        internal override void Refresh(PipeDX11 context, PipelineBindSlot<DeviceDX11, PipeDX11> slot)
        {
            if (State == null)
                State = new BlendState(context.Device.D3d, _desc);
        }

        private protected override void OnPipelineDispose()
        {
            DisposeObject(ref State);
        }

        public bool AlphaToCoverageEnable { get; }

        public bool IndependentBlendEnable { get; }

        /// <summary>
        /// Gets or sets the blend sample mask.
        /// </summary>
        public uint BlendSampleMask { get; }

        /// <summary>
        /// Gets or sets the blend factor.
        /// </summary>
        public Color4 BlendFactor { get; }
    }
}
