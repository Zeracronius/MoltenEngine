using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// The forward-rendering step.
    /// </summary>
    internal class ForwardStep : RenderStepBase
    {
        RenderSurface _surfaceScene;
        DepthStencilSurface _surfaceDepth;

        internal override void Initialize(RendererDX11 renderer)
        {
            _surfaceScene = renderer.GetSurface<RenderSurface>(MainSurfaceType.Scene);
            _surfaceDepth = renderer.GetDepthSurface();
        }

        public override void Dispose()
        { }

        internal override void Render(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, RenderChain.Context context, Timing time)
        {
            _surfaceScene.Clear(Color.Transparent);

            pipe.SetRenderSurface(_surfaceScene, 0);
            pipe.DepthSurface = _surfaceDepth;
            pipe.Rasterizer.SetViewports(camera.OutputSurface.Viewport);
            pipe.Rasterizer.SetScissorRectangle(camera.OutputSurface.Viewport.Bounds);

            StateConditions conditions = StateConditions.ScissorTest; // TODO expand
            conditions |= camera.OutputSurface.SampleCount > 1 ? StateConditions.Multisampling : StateConditions.None;

            pipe.BeginDraw(conditions);
            renderer.RenderSceneLayer(pipe, context.Layer, camera);
            pipe.EndDraw();
        }
    }
}
