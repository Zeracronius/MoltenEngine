﻿using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class StartStep : RenderStepBase
    {
        RenderSurface _surfaceScene;
        RenderSurface _surfaceNormals;
        RenderSurface _surfaceEmissive;
        DepthStencilSurface _surfaceDepth;

        internal override void Initialize(RendererDX11 renderer)
        {
            _surfaceScene = renderer.GetSurface<RenderSurface>(MainSurfaceType.Scene);
            _surfaceNormals = renderer.GetSurface<RenderSurface>(MainSurfaceType.Normals);
            _surfaceEmissive = renderer.GetSurface<RenderSurface>(MainSurfaceType.Emissive);
            _surfaceDepth = renderer.GetDepthSurface();
        }

        public override void Dispose() { }

        internal override void Render(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, RenderChain.Context context, Timing time)
        {
            pipe.SetRenderSurfaces(null);
            bool newSurface = renderer.ClearIfFirstUse(pipe, _surfaceScene, context.Scene.BackgroundColor);
            renderer.ClearIfFirstUse(pipe, _surfaceNormals, Color.White * 0.5f);
            renderer.ClearIfFirstUse(pipe, _surfaceEmissive, Color.Black);

            // Always clear the depth surface at the start of each scene unless otherwise instructed.
            // Will also be cleared if we've just switched to a previously un-rendered surface during this frame.
            if(!camera.Flags.HasFlag(RenderCameraFlags.DoNotClearDepth) || newSurface)
                _surfaceDepth.Clear(pipe, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil);
        }
    }
}
