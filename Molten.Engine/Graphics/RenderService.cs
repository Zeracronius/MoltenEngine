﻿using Molten.Collections;
using Molten.Graphics.Overlays;
using Molten.Threading;
using System;
using System.Collections.Generic;

namespace Molten.Graphics
{
    /// <summary>
    /// A base class that custom renderer implementations must inherit in order to be compatible with Molten engine, 
    /// as it provides basic functionality for interacting with the rest of the engine.
    /// </summary>
    public abstract class RenderService : EngineService
    {
        public static readonly Matrix4F DefaultView3D = Matrix4F.LookAtLH(new Vector3F(0, 0, -5), new Vector3F(0, 0, 0), Vector3F.UnitY);

        IRenderChain _chain;
        bool _disposeRequested;
        bool _shouldPresent;
        bool _surfaceResizeRequired;
        AntiAliasMode _requestedMultiSampleLevel = AntiAliasMode.None;
        internal AntiAliasMode MsaaLevel = AntiAliasMode.None;

        /// <summary>
        /// Creates a new instance of a <see cref="RenderService"/> sub-type.
        /// </summary>
        public RenderService()
        {
            Overlay = new OverlayProvider();
            Type test = this.GetType();
            Log.Log("Acquiring render chain");

            try
            {
                _chain = GetRenderChain();
                if (_chain != null)
                    Log.Log("Render chain acquired");
                else
                {
                    Log.Error("Render chain acquisition failed: Null chain");
                    throw new NullReferenceException("The provided render chain was null.");
                }
            }
            catch (Exception e)
            {
                Log.Error("Render chain acquisition failed. See exception details below");
                Log.Error(e, true);
                throw e;
            }
        }

        protected override ThreadingMode OnStart(ThreadManager threadManager)
        {
            _shouldPresent = true;
            return ThreadingMode.SeparateThread;
        }

        protected override void OnStop()
        {
            _shouldPresent = false;
        }

        /// <summary>
        /// Present's the renderer to it's bound output devices/surfaces.
        /// </summary>
        /// <param name="time"></param>
        protected override sealed void OnUpdate(Timing time)
        {
            /* CAMERA REFACTOR
             *  - The layer limit will be 32
             */

            /* DESIGN NOTES:
             *  - Store a hashset of materials used in each scene so that the renderer can set the "Common" buffer in one pass
             *  
             *  
             * MULTI-THREADING
             *  - Consider using 2+ worker threads to prepare a command list/deferred context from each scene, which can then
             *    be dispatched to the immediate context when all scenes have been processed
             *  - Avoid the above if any scenes interact with a render form surface at any point, since those can only be handled on the thread they're created on.
             *  
             *  - Consider using worker threads to:
             *      -- Sort front-to-back for rendering opaque objects (front-to-back reduces overdraw)
             *      -- Sort by buffer, material or textures (later in time)
             *      -- Sort back-to-front for rendering transparent objects (back-to-front reduces issues in alpha-blending)
             *  
             * 
             * 2D & UI Rendering:
             *  - Provide a sprite-batch for rendering 2D and UI
             *  - Prepare rendering of these on worker threads.
             */

            /* TODO: 
            *  Procedure:
            *      1) Calculate:
            *          a) Capture the current transform matrix of each object in the render tree
            *          b) Calculate the distance from the scene camera. Store on RenderData
            *          
            *      2) Sort objects by distance from camera:
            *          a) Sort objects into buckets inside RenderTree, front-to-back (reduce overdraw by drawing closest first).
            *          b) Only re-sort a bucket when objects are added or the camera moves
            *          c) While sorting, build up separate bucket list of objects with a transparent material sorted back-to-front (for alpha to work)
            *          
            *  Extras:
            *      3) Reduce z-sorting needed in (2) by adding scene-graph culling (quad-tree, octree or BSP) later down the line.
            *      4) Reduce (3) further by frustum culling the graph-culling results
            * 
            *  NOTES:
                - when SceneObject.IsVisible is changed, queue an Add or Remove operation on the RenderTree depending on visibility. This will remove it from culling/sorting.
            */

            if (_disposeRequested)
            {
                DisposeBeforeRender();
                return;
            }

            if (!_shouldPresent)
                return;

            Profiler.Begin();
            OnPrePresent(time);

            if (_requestedMultiSampleLevel != MsaaLevel)
            {
                // TODO re-create all internal surfaces/textures to match the new sample level.
                // TODO adjust rasterizer mode accordingly (multisample enabled/disabled).
                MsaaLevel = _requestedMultiSampleLevel;
                _surfaceResizeRequired = true;
            }

            // Perform all queued tasks before proceeding
            RendererTask task = null;
            while (Tasks.TryDequeue(out task))
                task.Process(this);

            // Perform preliminary checks on active scene data.
            // Also ensure the backbuffer is always big enough for the largest scene render surface.
            foreach (SceneRenderData data in Scenes)
            {
                data.ProcessChanges();

                foreach (RenderCamera camera in data.Cameras)
                {
                    camera.Skip = false;

                    if (camera.OutputSurface == null)
                    {
                        camera.Skip = true;
                        continue;
                    }

                    if (camera.OutputSurface.Width > BiggestWidth)
                    {
                        _surfaceResizeRequired = true;
                        BiggestWidth = camera.OutputSurface.Width;
                    }

                    if (camera.OutputSurface.Height > BiggestHeight)
                    {
                        _surfaceResizeRequired = true;
                        BiggestHeight = camera.OutputSurface.Height;
                    }
                }
            }

            // Update surfaces if dirty. This may involve resizing or changing their format.
            if (_surfaceResizeRequired)
            {
                OnRebuildSurfaces(BiggestWidth, BiggestHeight);
                _surfaceResizeRequired = false;
            }

            foreach (SceneRenderData sceneData in Scenes)
            {
                if (!sceneData.IsVisible)
                    continue;

                sceneData.PreRenderInvoke(this);
                sceneData.Profiler.Begin();

                // Sort cameras into ascending order-depth.
                sceneData.Cameras.Sort((a, b) =>
                {
                    if (a.OrderDepth > b.OrderDepth)
                        return 1;
                    else if (a.OrderDepth < b.OrderDepth)
                        return -1;
                    else
                        return 0;
                });

                OnPreRenderScene(sceneData, time);
                foreach (RenderCamera camera in sceneData.Cameras)
                {
                    if (camera.Skip)
                        continue;

                    OnPreRenderCamera(sceneData, camera, time);
                    camera.Profiler.Begin();

                    LayerRenderData layer;
                    for (int i = 0; i < sceneData.Layers.Count; i++)
                    {
                        layer = sceneData.Layers[i];
                        int layerBitVal = 1 << i;
                        if ((camera.LayerMask & layerBitVal) == layerBitVal)
                            continue;

                        _chain.Build(sceneData, layer, camera);
                        _chain.Render(sceneData, layer, camera, time);
                    }

                    camera.Profiler.End(time);
                    Profiler.Accumulate(camera.Profiler.Previous);
                    sceneData.Profiler.Accumulate(camera.Profiler.Previous);
                    OnPostRenderCamera(sceneData, camera, time);
                }
                OnPostRenderScene(sceneData, time);

                sceneData.Profiler.End(time);
                sceneData.PostRenderInvoke(this);
            }

            // Present all output surfaces
            OutputSurfaces.For(0, 1, (index, surface) =>
            {
                surface.Present();
                return false;
            });

            OnPostPresent(time);
            Profiler.End(time);
        }

        /// <summary>
        /// Occurs when a graphics adapter needs to be acquired, before the renderer is initialized.
        /// </summary>
        /// <param name="settings"></param>
        protected abstract void OnInitializeAdapter(GraphicsSettings settings);

        /// <summary>
        /// Occurs when the renderer is being initialized.
        /// </summary>
        /// <param name="settings"></param>
        protected override void OnInitialize(EngineSettings settings)
        {
            try
            {
                OnInitializeAdapter(settings.Graphics);
                Log.Log($"Initialized adapter");
            }
            catch (Exception ex)
            {
                Log.Log("Failed to initialize renderer");
                Log.Error(ex, true);
            }

            settings.Log(Log, "Graphics");
            MsaaLevel = _requestedMultiSampleLevel = MsaaLevel;
            settings.Graphics.MSAA.OnChanged += MSAA_OnChanged;
        }

        private void MSAA_OnChanged(AntiAliasMode oldValue, AntiAliasMode newValue)
        {
            _requestedMultiSampleLevel = newValue;
        }

        public SceneRenderData CreateRenderData()
        {
            SceneRenderData rd = OnCreateRenderData();
            RendererAddScene task = RendererAddScene.Get();
            task.Data = rd;
            PushTask(task);
            return rd;
        }

        protected abstract SceneRenderData OnCreateRenderData();

        public void DestroyRenderData(SceneRenderData data)
        {
            RendererRemoveScene task = RendererRemoveScene.Get();
            task.Data = data;
            PushTask(task);
        }

        public void PushTask(RendererTask task)
        {
            Tasks.Enqueue(task);
        }

        protected abstract IRenderChain GetRenderChain();

        /// <summary>
        /// Occurs when the render engine detects changes which usually require render surfaces to be rebuilt, such as the game window being resized, or certain graphics settings being changed.
        /// </summary>
        /// <param name="requiredWidth">The new width required by the render engine.</param>
        /// <param name="requiredHeight">The new height required by the render engine.</param>
        protected abstract void OnRebuildSurfaces(uint requiredWidth, uint requiredHeight);

        /// <summary>
        /// Occurs before the render engine begins rendering all of the active scenes to be output to the user.
        /// </summary>
        /// <param name="time">A timing instance.</param>
        protected abstract void OnPrePresent(Timing time);

        protected abstract void OnPreRenderScene(SceneRenderData sceneData, Timing time);

        protected abstract void OnPostRenderScene(SceneRenderData sceneData, Timing time);

        protected abstract void OnPreRenderCamera(SceneRenderData sceneData, RenderCamera camera, Timing time);

        protected abstract void OnPostRenderCamera(SceneRenderData sceneData, RenderCamera camera, Timing time);

        /// <summary>
        /// Occurs after render presentation is completed and profiler timing has been finalized for the current frame. Useful if you need to do some per-frame cleanup/resetting.
        /// </summary>
        /// <param name="time">A timing instance.</param>
        protected abstract void OnPostPresent(Timing time);

        /// <summary>
        /// Occurs when the current <see cref="RenderService"/> instance/implementation is being disposed.
        /// </summary>
        protected override sealed void OnDispose()
        {
            _disposeRequested = true;
        }

        protected void DisposeBeforeRender()
        {
            base.OnDispose();

            OutputSurfaces.For(0, 1, (index, surface) =>
            {
                surface.Dispose();
                return false;
            });

            OnDisposeBeforeRender();

            Log.Dispose();
        }
        protected abstract void OnDisposeBeforeRender();

        /// <summary>
        /// Gets profiling data attached to the renderer.
        /// </summary>
        public RenderProfiler Profiler { get; } = new RenderProfiler();

        /// <summary>
        /// Gets the display manager bound to the renderer.
        /// </summary>
        public abstract IDisplayManager DisplayManager { get; }

        /// <summary>
        /// Gets the <see cref="ResourceFactory"/> bound to the renderer.
        /// This is responsible for creating and destroying graphics resources, such as buffers, textures and surfaces.
        /// </summary>
        public abstract ResourceFactory Resources { get; }

        /// <summary>
        /// Gets the compute manager attached to the current renderer.
        /// </summary>
        public abstract IComputeManager Compute { get; }

        /// <summary>
        /// Gets a list of all the output <see cref="ISwapChainSurface"/> instances attached to the renderer. These are automatically presented to the graphics device by the renderer, if active.
        /// </summary>
        public ThreadedList<ISwapChainSurface> OutputSurfaces { get; } = new ThreadedList<ISwapChainSurface>();

        /// <summary>
        /// Gets a list of all the scenes current attached to the renderer.
        /// </summary>
        protected internal List<SceneRenderData> Scenes { get; } = new List<SceneRenderData>();

        private ThreadedQueue<RendererTask> Tasks { get; } = new ThreadedQueue<RendererTask>();

        /// <summary>
        /// Gets the width of the biggest render surface used so far.
        /// </summary>
        protected uint BiggestWidth { get; private set; } = 1;

        /// <summary>
        /// Gets the height of the biggest render surface used so far.
        /// </summary>
        protected uint BiggestHeight { get; private set; } = 1;

        /// <summary>
        /// Gets the renderer's <see cref="OverlayProvider"/> implementation.
        /// </summary>
        public OverlayProvider Overlay { get; }
    }
}
