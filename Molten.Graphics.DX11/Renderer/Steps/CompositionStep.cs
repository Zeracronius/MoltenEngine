﻿using System;
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
        Material _matCompose;
        IShaderValue _valLighting;
        IShaderValue _valEmissive;

        internal override void Initialize(RendererDX11 renderer)
        {
            _surfaceScene = renderer.GetSurface<RenderSurface>(MainSurfaceType.Scene);
            _surfaceLighting = renderer.GetSurface<RenderSurface>(MainSurfaceType.Lighting);
            _surfaceEmissive = renderer.GetSurface<RenderSurface>(MainSurfaceType.Emissive);

            string namepace = "Molten.Graphics.Assets.gbuffer_compose.mfx";

            ShaderCompileResult result = renderer.Resources.LoadEmbeddedShader("Molten.Graphics.Assets", "gbuffer_compose.mfx");
            _matCompose = result[ShaderClassType.Material, "gbuffer-compose"] as Material;

            _valLighting = _matCompose["mapLighting"];
            _valEmissive = _matCompose["mapEmissive"];

            _dummyData = new ObjectRenderData();
            _orthoCamera = new RenderCamera(RenderCameraMode.Orthographic);
        }

        public override void Dispose()
        {
            _matCompose.Dispose();
        }

        internal override void Render(RendererDX11 renderer, RenderCamera camera, RenderChain.Context context, Timing time)
        {
            _orthoCamera.OutputSurface = camera.OutputSurface;

            RectangleF vpBounds = camera.OutputSurface.Viewport.Bounds;
            Device device = renderer.Device;

            context.CompositionSurface.Clear(context.Scene.BackgroundColor);
            device.State.ResetRenderSurfaces();
            device.State.SetRenderSurface(context.CompositionSurface, 0);
            device.State.DepthSurface.Value = null;
            device.State.DepthWriteOverride = GraphicsDepthWritePermission.Disabled;
            device.State.SetViewports(camera.OutputSurface.Viewport);
            device.State.SetScissorRectangle((Rectangle)vpBounds);

            StateConditions conditions = StateConditions.ScissorTest;
            conditions |= camera.OutputSurface.SampleCount > 1 ? StateConditions.Multisampling : StateConditions.None;

            _valLighting.Value = _surfaceLighting;
            _valEmissive.Value = _surfaceEmissive;

            ITexture2D sourceSurface = context.HasComposed ? context.PreviousComposition : _surfaceScene;

            renderer.Device.BeginDraw(conditions); // TODO correctly use pipe + conditions here.
            renderer.SpriteBatcher.Draw(sourceSurface, vpBounds, Vector2F.Zero, vpBounds.Size, Color.White, 0, Vector2F.Zero, _matCompose, 0);
            renderer.SpriteBatcher.Flush(device, _orthoCamera, _dummyData);
            renderer.Device.EndDraw();

            context.SwapComposition();
        }
    }
}
