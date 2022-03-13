﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal unsafe class BlendBinder : ContextSlotBinder<GraphicsBlendState>
    {
        internal override void Bind(ContextSlot<GraphicsBlendState> slot, GraphicsBlendState value)
        {
            value = value ?? slot.Context.Device.BlendBank.GetPreset(BlendPreset.Default);
            Color4 tmp = value.BlendFactor;
            slot.Context.Native->OMSetBlendState(value, (float*)&tmp, value.BlendSampleMask);
        }

        internal override void Unbind(ContextSlot<GraphicsBlendState> slot, GraphicsBlendState value)
        {
            slot.Context.Native->OMSetBlendState(null, null, 0);
        }
    }
}
