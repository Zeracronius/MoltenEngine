﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class RenderTargetView : ResourceView<ID3D11RenderTargetView, RenderTargetViewDesc>
    {
        internal RenderTargetView(Device device) : base(device) { }

        protected override unsafe void OnCreateView(ID3D11Resource* resource, ref RenderTargetViewDesc desc, ref ID3D11RenderTargetView* view)
        {
            Device.NativeDevice->CreateRenderTargetView(resource, ref desc, ref view);
        }
    }
}
