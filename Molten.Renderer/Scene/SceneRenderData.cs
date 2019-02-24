﻿using Molten.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public delegate void SceneRenderDataHandler(MoltenRenderer renderer, SceneRenderData data);

    /// <summary>
    /// A class for storing renderer-specific information about a scene.
    /// </summary>
    public abstract class SceneRenderData : EngineObject
    {
        /// <summary>
        /// Occurs just before the scene is about to be rendered.
        /// </summary>
        public event SceneRenderDataHandler OnPreRender;

        /// <summary>
        /// Occurs just after the scene has been rendered.
        /// </summary>
        public event SceneRenderDataHandler OnPostRender;

        /// <summary>
        /// If true, the scene will be rendered.
        /// </summary>
        public bool IsVisible = true;

        /// <summary>
        /// The background color of the scene.
        /// </summary>
        public Color BackgroundColor = new Color(20,20,20,255);

        /// <summary>
        /// The ambient light color.
        /// </summary>
        public Color AmbientLightColor = Color.Black;

        public List<LayerRenderData> Layers = new List<LayerRenderData>();
        protected readonly ThreadedQueue<RenderSceneChange> _pendingChanges = new ThreadedQueue<RenderSceneChange>();

        public abstract LayerRenderData CreateLayerData(string name);

        public void AddLayer(LayerRenderData data)
        {
            RenderLayerAdd change = RenderLayerAdd.Get();
            change.LayerData = data;
            change.SceneData = this;
            _pendingChanges.Enqueue(change);
        }

        public void RemoveLayer(LayerRenderData data)
        {
            RenderLayerRemove change = RenderLayerRemove.Get();
            change.LayerData = data;
            change.SceneData = this;
            _pendingChanges.Enqueue(change);
        }

        public void ReorderLayer(LayerRenderData data, ReorderMode mode)
        {
            RenderLayerReorder change = RenderLayerReorder.Get();
            change.LayerData = data;
            change.SceneData = this;
            change.Mode = mode;
            _pendingChanges.Enqueue(change);
        }

        public void AddObject(RenderCamera obj)
        {
            AddCamera change = AddCamera.Get();
            change.Camera = obj;
            change.Data = this;
            _pendingChanges.Enqueue(change);
        }

        public void RemoveObject(RenderCamera obj)
        {
            RemoveCamera change = RemoveCamera.Get();
            change.Camera = obj;
            change.Data = this;
            _pendingChanges.Enqueue(change);
        }

        public abstract void AddObject(IRenderable obj, ObjectRenderData renderData, LayerRenderData layer);

        public abstract void RemoveObject(IRenderable obj, ObjectRenderData renderData, LayerRenderData layer);

        internal void ProcessChanges()
        {
            while (_pendingChanges.TryDequeue(out RenderSceneChange change))
                change.Process();
        }

        /// <summary>
        /// Invokes <see cref="OnPreRender"/> event.
        /// </summary>
        public void PreRenderInvoke(MoltenRenderer renderer) => OnPreRender?.Invoke(renderer, this);

        /// <summary>
        /// Invokes <see cref="OnPostRender"/> event.
        /// </summary>
        public void PostRenderInvoke(MoltenRenderer renderer) => OnPostRender?.Invoke(renderer, this);

        /* TODO:
        *  - Edit PointLights and CapsuleLights.Data directly in light scene components (e.g. PointLightComponent).
        *  - Renderer will upload the latest data to the GPU 
        */

        public LightList PointLights { get; } = new LightList(100, 100);

        public LightList CapsuleLights { get; } = new LightList(50, 100);

        public List<RenderCamera> Cameras { get; } = new List<RenderCamera>();

        public RenderProfiler Profiler { get; } = new RenderProfiler();

        /// <summary>
        /// Gets or sets the skybox cube-map texture.
        /// </summary>
        public ITextureCube SkyboxTexture { get; set; }
    }

    public class SceneRenderData<R> : SceneRenderData
        where R: class, IRenderable
    {
        public override LayerRenderData CreateLayerData(string name)
        {
            return new LayerRenderData<R>()
            {
                Name = name,
            };
        }

        public override void AddObject(IRenderable obj, ObjectRenderData renderData, LayerRenderData layer)
        {
            RenderableAdd<R> change = RenderableAdd<R>.Get();
            change.Renderable = obj as R;
            change.Data = renderData;
            change.LayerData = layer as LayerRenderData<R>;
            _pendingChanges.Enqueue(change);
        }

        public override void RemoveObject(IRenderable obj, ObjectRenderData renderData, LayerRenderData layer)
        {
            RenderableRemove<R> change = RenderableRemove<R>.Get();
            change.Renderable = obj as R;
            change.Data = renderData;
            change.LayerData = layer as LayerRenderData<R>;
            _pendingChanges.Enqueue(change);
        }
    }
}
