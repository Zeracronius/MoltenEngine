using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class FinalizeStep : RenderStepBase
    {
        RenderCamera _orthoCamera;
        ObjectRenderData _dummyData;

        RenderSurface _surfaceScene;

        internal override void Initialize(RendererDX11 renderer)
        {
            _surfaceScene = renderer.GetSurface<RenderSurface>(MainSurfaceType.Scene);

            _dummyData = new ObjectRenderData();
            _orthoCamera = new RenderCamera(RenderCameraMode.Orthographic);
        }

        public override void Dispose()
        {

        }

        internal override void Render(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, RenderChain.Context context, Timing time)
        {
            _orthoCamera.OutputSurface = camera.OutputSurface;

            Rectangle bounds = new Rectangle(0, 0, camera.OutputSurface.Width, camera.OutputSurface.Height);
            RenderSurface finalSurface = camera.OutputSurface as RenderSurface;
            if (!camera.HasFlags(RenderCameraFlags.DoNotClear))
                renderer.ClearIfFirstUse(pipe, finalSurface, context.Scene.BackgroundColor);

            pipe.SetRenderSurfaces(null);
            pipe.SetRenderSurface(finalSurface, 0);
            pipe.DepthSurface = null;
            pipe.DepthWriteOverride = GraphicsDepthWritePermission.Disabled;
            pipe.Rasterizer.SetViewports(camera.OutputSurface.Viewport);
            pipe.Rasterizer.SetScissorRectangle(camera.OutputSurface.Viewport.Bounds);

            StateConditions conditions = StateConditions.ScissorTest;
            conditions |= camera.OutputSurface.SampleCount > 1 ? StateConditions.Multisampling : StateConditions.None;

            renderer.Device.BeginDraw(conditions); // TODO correctly use pipe + conditions here.

            ITexture2D sourceSurface = context.HasComposed ? context.PreviousComposition : _surfaceScene;
            pipe.SpriteBatcher.Draw(sourceSurface, bounds, Vector2F.Zero, camera.OutputSurface.Viewport.Bounds.Size, Color.White, 0, Vector2F.Zero, null, 0);

            if (camera.HasFlags(RenderCameraFlags.ShowOverlay))
                renderer.Overlay.Render(time, pipe.SpriteBatcher, renderer.Profiler, context.Scene.Profiler, camera);

            pipe.SpriteBatcher.Flush(pipe, _orthoCamera, _dummyData);
            renderer.Device.EndDraw();
        }
    }
}
