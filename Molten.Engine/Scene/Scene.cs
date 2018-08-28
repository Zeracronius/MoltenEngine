﻿using Molten.Collections;
using Molten.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    /// <summary>Manages and controls a scene composed of 3D and 2D objects, while also feeding the needed data to a renderer if available. <para/>
    /// Multiple scenes can render to the same output camera if needed. This allows scenes to easily be onto the same surface. Ordering methods are provided for this purpose.</summary>
    public partial class Scene : EngineObject
    {
        internal SceneRenderData RenderData;
        internal List<ISceneObject> Objects;
        internal List<IRenderable2D> Renderables2d;
        internal HashSet<IUpdatable> Updatables;
        internal List<ICursorAcceptor> InputAcceptors;

        ThreadedQueue<SceneChange> _pendingChanges;
        RenderDebugOverlay _renderOverlay;

        /// <summary>Creates a new instance of <see cref="Scene"/></summary>
        /// <param name="name">The name of the scene.</param>
        /// <param name="engine">The engine instance to which the scene will be bound.</param>
        internal Scene(string name, Engine engine, SceneRenderFlags flags)
        {
            Name = name;
            Engine = engine;
            RenderData = engine.Renderer.CreateRenderData();
            RenderData.Flags = flags;

            engine.AddScene(this);
            Objects = new List<ISceneObject>();
            Renderables2d = new List<IRenderable2D>();
            Updatables = new HashSet<IUpdatable>();
            InputAcceptors = new List<ICursorAcceptor>();
            _pendingChanges = new ThreadedQueue<SceneChange>();
            _renderOverlay = new RenderDebugOverlay(RenderData.DebugOverlay);

            engine.Log.WriteLine($"Created scene '{name}'");
        }

        /// <summary>
        /// Brings the scene to the front of the render stack. The scene will be rendered on top of all other scenes on the same <see cref="IRenderSurface"/>.
        /// </summary>
        public void BringToFront()
        {
            Engine.Renderer?.BringToFront(RenderData);
        }

        /// <summary>
        /// Sends the scene to the back of the render stack. The scene will be rendered behind all other scenes on the same <see cref="IRenderSurface"/>.
        /// </summary>
        public void SendToBack()
        {
            Engine.Renderer?.SendToBack(RenderData);
        }

        /// <summary>
        /// Pushes the scene forward by one ID in the render stack. The scene will be rendered on top of any other scenes that come before it in the render stack.
        /// </summary>
        public void PushForward()
        {
            Engine.Renderer?.PushForward(RenderData);
        }

        /// <summary>
        /// Pushes the scene back by one ID in the render stack. The scene will be rendered on top of any other scenes that come before it in the render stack.
        /// </summary>
        public void PushBackward()
        {
            Engine.Renderer?.PushBackward(RenderData);
        }

        /// <summary>Adds a <see cref="SceneObject"/> to the scene.</summary>
        /// <param name="obj">The object to be added.</param>
        public void AddObject(ISceneObject obj)
        {
            SceneAddObject change = SceneAddObject.Get();
            change.Object = obj;
            _pendingChanges.Enqueue(change);
        }

        /// <summary>Removes a <see cref="SceneObject"/> from the scene.</summary>
        /// <param name="obj">The object to be removed.</param>
        public void RemoveObject(ISceneObject obj)
        {
            SceneRemoveObject change = SceneRemoveObject.Get();
            change.Object = obj;
            _pendingChanges.Enqueue(change);
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="time">A <see cref="Timing"/> instance.</param>
        internal void Update(Timing time)
        {
            while (_pendingChanges.TryDequeue(out SceneChange change))
                change.Process(this);

            foreach (IUpdatable up in Updatables)
                up.Update(time);
        }

        protected override void OnDispose()
        {
            Engine.RemoveScene(this);
            Engine.Renderer.DestroyRenderData(RenderData);
            Engine.Log.WriteLine($"Destroyed scene '{Name}'");
            base.OnDispose();
        }

        /// <summary>Gets or sets whether or not the scene is updated.</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>Gets or sets whether the scene is rendered.</summary>
        public bool IsVisible
        {
            get => RenderData.IsVisible;
            set => RenderData.IsVisible = value;
        }

        /// <summary>Gets the name of the scene.</summary>
        public string Name { get; private set; }

        /// <summary>Gets or sets the scene's out camera. This acts as an eye when rendering the scene, allowing it to be viewed from the perspective of the camera.
        /// Scenes without a camera will not be updated or rendered.</summary>
        public ICamera OutputCamera
        {
            get => RenderData.Camera;
            set => RenderData.Camera = value;
        }

        /// <summary>Gets the <see cref="Engine"/> instance that the <see cref="Scene"/> is bound to.</summary>
        public Engine Engine { get; private set; }

        /// <summary>Gets or sets the background color of the scene.</summary>
        public Color BackgroundColor
        {
            get => RenderData.BackgroundColor;
            set => RenderData.BackgroundColor = value;
        }

        /// <summary>
        /// Gets or sets the ambient light color of the scene.
        /// </summary>
        public Color AmbientColor
        {
            get => RenderData.AmbientLightColor;
            set => RenderData.AmbientLightColor = value;
        }

        /// <summary>
        /// Gets or sets the scene's render flags.
        /// </summary>
        public SceneRenderFlags RenderFlags
        {
            get => RenderData.Flags;
            set => RenderData.Flags = value;
        }

        /// <summary>
        /// Gets the scene's debug overlay. 
        /// The overlay can be added to another scene as an <see cref="IRenderable2D"/> object if you want to render the overlay into a different scene.
        /// </summary>
        public RenderDebugOverlay DebugOverlay => _renderOverlay;

        /// <summary>
        /// Gets or sets the input bounds of the current <see cref="Scene"/> instance. A cursor must be within these bounds for the scene to receive cursor input. <para/>
        /// Any input objects of the current <see cref="Scene"/> will receive cursor coordinates relative to these bounds.
        /// </summary>
        public Rectangle InputBounds { get; set; }
    }
}
