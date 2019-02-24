using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class CompositionStep : RenderStepBase
    {
        RenderCamera _orthoCamera;
        ObjectRenderData _dummyData;
        RenderSurface _surfaceScene;
        RenderSurface _surfaceLighting;
        RenderSurface _surfaceEmissive;
        IMaterial _matCompose;
        IShaderValue _valLighting;
        IShaderValue _valEmissive;

        internal override void Initialize(RendererDX11 renderer)
        {
            _surfaceScene = renderer.GetSurface<RenderSurface>(MainSurfaceType.Scene);
            _surfaceLighting = renderer.GetSurface<RenderSurface>(MainSurfaceType.Lighting);
            _surfaceEmissive = renderer.GetSurface<RenderSurface>(MainSurfaceType.Emissive);

            string nsCompose = "Molten.Graphics.Assets.gbuffer_compose.mfx";
            ShaderCompileResult result = renderer.ShaderCompiler.CompileEmbedded(nsCompose);
            _matCompose = result["material", "gbuffer-compose"] as Material;

            _valLighting = _matCompose["mapLighting"];
            _valEmissive = _matCompose["mapEmissive"];

            _dummyData = new ObjectRenderData();
            _orthoCamera = new RenderCamera(RenderCameraMode.Orthographic);
        }

        public override void Dispose()
        {
            _matCompose.Dispose();
        }

        internal override void Render(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, RenderChain.Context context, Timing time)
        {
            _orthoCamera.OutputSurface = camera.OutputSurface;
            Rectangle bounds = camera.OutputSurface.Viewport.Bounds;
            context.CompositionSurface.Clear(context.Scene.BackgroundColor);

            pipe.UnsetRenderSurfaces();
            pipe.SetRenderSurface(context.CompositionSurface, 0);
            pipe.DepthSurface = null;
            pipe.DepthWriteOverride = GraphicsDepthWritePermission.Disabled;
            pipe.Rasterizer.SetViewports(camera.OutputSurface.Viewport);
            pipe.Rasterizer.SetScissorRectangle(bounds);

            StateConditions conditions = StateConditions.ScissorTest;
            conditions |= camera.OutputSurface.SampleCount > 1 ? StateConditions.Multisampling : StateConditions.None;

            _valLighting.Value = _surfaceLighting;
            _valEmissive.Value = _surfaceEmissive;

            ITexture2D sourceSurface = context.HasComposed ? context.PreviousComposition : _surfaceScene;

            pipe.BeginDraw(conditions); // TODO correctly use pipe + conditions here.
            pipe.SpriteBatcher.Draw(sourceSurface, bounds, Vector2F.Zero, bounds.Size, Color.White, 0, Vector2F.Zero, _matCompose, 0);
            pipe.SpriteBatcher.Flush(pipe, _orthoCamera, _dummyData);
            pipe.EndDraw();

            context.SwapComposition();
        }
    }
}
