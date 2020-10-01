using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>Stores a depth-stencil state for use with a <see cref="PipeDX11"/>.</summary>
    internal class GraphicsDepthState : PipelineObject<DeviceDX11, PipeDX11>, IEquatable<GraphicsDepthState>
    {
        internal DepthStencilState State;
        DepthStencilStateDescription _desc;

        internal GraphicsDepthState(DeviceDX11 device, GraphicsDepthState source) : base(device)
        {
            _desc = source._desc;
            StencilReference = source.StencilReference;
            WritePermission = source.WritePermission;
        }

        internal GraphicsDepthState(DeviceDX11 device) : this(device, ShaderDepthStencilDefinition.Presets[DepthStencilPreset.Default]) { }

        internal GraphicsDepthState(DeviceDX11 device, ShaderDepthStencilDefinition definition) : base(device)
        {
            StencilReference = definition.StencilReference;
            WritePermission = definition.WritePermission;
            _desc = new DepthStencilStateDescription()
            {
                BackFace = new DepthStencilOperationDescription()
                {
                    Comparison = (Comparison)definition.FrontFace.Comparison,
                    DepthFailOperation = (StencilOperation)definition.FrontFace.DepthFailOperation,
                    FailOperation= (StencilOperation)definition.FrontFace.FailOperation,
                    PassOperation = (StencilOperation)definition.FrontFace.PassOperation,
                },

                FrontFace = new DepthStencilOperationDescription()
                {
                    Comparison = (Comparison)definition.BackFace.Comparison,
                    DepthFailOperation = (StencilOperation)definition.BackFace.DepthFailOperation,
                    FailOperation = (StencilOperation)definition.BackFace.FailOperation,
                    PassOperation = (StencilOperation)definition.BackFace.PassOperation,
                },

                DepthComparison = (Comparison)definition.DepthFunc,
                DepthWriteMask = (DepthWriteMask)definition.DepthWriteMask,
                IsDepthEnabled = definition.IsDepthEnabled,
                IsStencilEnabled = definition.IsStencilEnabled,
                StencilReadMask = definition.StencilReadMask,
                StencilWriteMask = definition.StencilWriteMask,
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphicsDepthState other)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(GraphicsDepthState other)
        {
            if (!CompareOperation(ref _desc.BackFace, ref other._desc.BackFace) || !CompareOperation(ref _desc.FrontFace, ref other._desc.FrontFace))
                return false;

            return _desc.DepthComparison == other._desc.DepthComparison &&
                _desc.IsDepthEnabled == other._desc.IsDepthEnabled &&
                _desc.IsStencilEnabled == other._desc.IsStencilEnabled &&
                _desc.StencilReadMask == other._desc.StencilReadMask &&
                _desc.StencilWriteMask == other._desc.StencilWriteMask;
        }

        private static bool CompareOperation(ref DepthStencilOperationDescription op, ref DepthStencilOperationDescription other)
        {
            return op.Comparison == other.Comparison &&
                op.DepthFailOperation == other.DepthFailOperation &&
                op.FailOperation == other.FailOperation &&
                op.PassOperation == other.PassOperation;
        }

        internal override void Refresh(PipeDX11 pipe, PipelineBindSlot<DeviceDX11, PipeDX11> slot)
        {
            if(State == null)
                State = new DepthStencilState(pipe.Device.D3d, _desc);
        }

        private protected override void OnPipelineDispose()
        {
            DisposeObject(ref State);
        }

        /// <summary>Gets or sets the stencil reference value. The default value is 0.</summary>
        internal int StencilReference { get; set; }

        /// <summary>
        /// Gets or sets the depth write permission. the default value is <see cref="GraphicsDepthWritePermission.Enabled"/>.
        /// </summary>
        internal GraphicsDepthWritePermission WritePermission { get; set; }
    }
}
