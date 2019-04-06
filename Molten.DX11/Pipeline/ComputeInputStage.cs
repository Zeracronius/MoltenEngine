﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ComputeInputStage : ShaderStage<ComputeTask>
    {
        ShaderStep<ComputeShader, ComputeShaderStage, ComputeTask> _cStage;
        PipelineBindSlot<PipelineShaderObject, DeviceDX11, PipeDX11>[] _slotUAVs;

        internal ComputeInputStage(PipeDX11 pipe) : base(pipe)
        {
            _cStage = CreateStep<ComputeShader, ComputeShaderStage>(pipe.Context.ComputeShader, (stage, composition) => stage.Set(composition.RawShader));
            _slotUAVs = new PipelineBindSlot<PipelineShaderObject, DeviceDX11, PipeDX11>[Device.Features.MaxUnorderedAccessViews];

            for (int i = 0; i < Device.Features.MaxUnorderedAccessViews; i++)
            {
                _slotUAVs[i] = new PipelineBindSlot<PipelineShaderObject, DeviceDX11, PipeDX11>(this, i);
                _slotUAVs[i].OnObjectForcedUnbind += ComputeStage_OnBoundObjectDisposed;
            }
        }

        private void ComputeStage_OnBoundObjectDisposed(PipelineBindSlot<DeviceDX11, PipeDX11> slot, PipelineDisposableObject obj)
        {
            _cStage.RawStage.SetUnorderedAccessView(slot.SlotID, null);
        }

        private void _cStage_OnSetShader(ComputeTask shader, ShaderComposition<ComputeShader> composition, ComputeShaderStage shaderStage)
        {
            // Bind all UAV resourcess
            RWVariable u = null;
            for (int i = 0; i < composition.UnorderedAccessIds.Count; i++)
            {
                int slotID = composition.UnorderedAccessIds[i];
                u = shader.UAVs[slotID];
                if (u == null)
                    continue;

                bool uavChanged = _slotUAVs[slotID].Bind(Pipe, u.Resource, PipelineBindType.Output);
                if (uavChanged)
                {
                    if (u.UnorderedResource.UAV == null)
                        shaderStage.SetUnorderedAccessView(slotID, null);
                    else
                        shaderStage.SetUnorderedAccessView(slotID, u.UnorderedResource.UAV, -1);
                }
            }

            shaderStage.Set(composition.RawShader);
        }

        internal void Dispatch(int groupsX, int groupsY, int groupsZ)
        {
            if (_shader.Bind())
            {
                if (_shader.BoundValue == null || IsValid == false)
                {
                    // TODO unbind all currently bound resources.
                    return;
                }
                else
                {
                    _cStage.Refresh(_shader.BoundValue, _shader.BoundValue.Composition);

                    // Ensure dispatch is within supported range.
                    int maxZ = Device.Features.Compute.MaxDispatchZDimension;
                    int maxXY = Device.Features.Compute.MaxDispatchXYDimension;

                    if (groupsZ > maxZ)
                    {
#if DEBUG
                        Pipe.Log.Write("Unable to dispatch compute shader. Z dimension (" + groupsZ + ") is greater than supported (" + maxZ + ").");
#endif
                        return;
                    }
                    else if (groupsX > maxXY)
                    {
#if DEBUG
                        Pipe.Log.Write("Unable to dispatch compute shader. X dimension (" + groupsX + ") is greater than supported (" + maxXY + ").");
#endif
                        return;
                    }
                    else if (groupsY > maxXY)
                    {
#if DEBUG
                        Pipe.Log.Write("Unable to dispatch compute shader. Y dimension (" + groupsY + ") is greater than supported (" + maxXY + ").");
#endif
                        return;
                    }

                    // TODO have this processed during the presentation call of each graphics pipe.

                    Pipe.Context.Dispatch(groupsX, groupsY, groupsZ);
                }
            }
            else if (_shader.BoundValue != null)
            {                
                //RefreshSlots();
            }
        }
    }
}
