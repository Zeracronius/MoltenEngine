﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class SpriteRendererDX11 : Renderable, ISpriteRenderer
    {
        internal SpriteRendererDX11(DeviceDX11 device, Action<SpriteBatcher> callback) : base(device)
        {
            Callback = callback;
        }

        public Action<SpriteBatcher> Callback { get; set; }

        private protected override void OnRender(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, ObjectRenderData data)
        {
            Callback?.Invoke(pipe.SpriteBatcher);
            pipe.SpriteBatcher.Flush(pipe, camera, data);
        }
    }
}
