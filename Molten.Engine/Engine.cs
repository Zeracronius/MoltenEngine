﻿using Molten.Collections;
using Molten.Graphics;
using Molten.Input;
using Molten.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    /// <summary>Stone Bolt engine. You only need one instance.</summary>
    public class Engine : IDisposable
    {
        EngineSettings _settings;
        Logger _log;
        IRenderer _renderer;
        ThreadManager _threadManager;
        EngineThread _threadRenderer;
        ContentManager _content;
        InputManager _input;

        internal List<Scene> Scenes;

        ThreadedQueue<EngineTask> _taskQueue;

        /// <summary>
        /// Occurs right after the display manager has detected active display <see cref="IDisplayOutput"/>. Here you can change the output configuration before it is passed
        /// down to the graphics and rendering chain.
        /// </summary>
        event DisplayManagerEvent OnAdapterInitialized;

        public Engine(EngineSettings settings = null)
        {
            _settings = settings ?? new EngineSettings();
            _settings.Load();

            _log = Logger.Get();
            _log.AddOutput(new LogFileWriter("engine_log{0}.txt"));
            _log.WriteDebugLine("Engine Instantiated");
            _threadManager = new ThreadManager(this, _log);
            _taskQueue = new ThreadedQueue<EngineTask>();
            _content = new ContentManager(_log, this, settings.ContentRootDirectory, null, settings.ContentWorkerThreads);
            _input = new InputManager(_log);
            Scenes = new List<Scene>();
        }
        
        public void LoadRenderer()
        {
            if (_renderer != null)
            {
                _log.WriteLine("Attempted to load renderer when one is already loaded!");
                return;
            }

            // Load renderer library
            RenderLoader renderLoader = new RenderLoader(_log, _settings.Graphics);
            _renderer = renderLoader.GetRenderer();
            OnAdapterInitialized?.Invoke(_renderer.DisplayManager);
            _renderer.InitializeRenderer(_settings.Graphics);
        }

        public void AddScene(Scene scene)
        {
            EngineAddScene task = EngineAddScene.Get();
            task.Scene = scene;
            _taskQueue.Enqueue(task);
        }

        public void RemoveScene(Scene scene)
        {
            EngineRemoveScene task = EngineRemoveScene.Get();
            task.Scene = scene;
            _taskQueue.Enqueue(task);
        }

        public SceneObject CreateObject()
        {
            return new SceneObject(this);
        }

        /// <summary>Starts the renderer thread.</summary>
        public void StartRenderer()
        {
            if (_renderer == null)
            {
                _log.WriteLine("A renderer has not be loaded. Unable to start renderer");
                _log.WriteDebugLine("Please ensure Engine.LoadRenderer() was called and a valid renderer library was provided.");
                return;
            }

            if (_threadRenderer != null)
            {
                _log.WriteLine("Ignored attempt to start renderer thread while already running");
                return;
            }

            _threadRenderer = _threadManager.SpawnThread("Renderer_main", true, false, (time) =>
            {
                _renderer.Present(time);
            });

            _log.WriteLine("Renderer thread started");
        }

        /// <summary>Stops the renderer thread.</summary>
        public void StopRenderer()
        {
            if (_renderer == null || _threadRenderer == null)
            {
                _log.WriteLine("Ignored attempt to stop renderer while not running");
                return;
            }

            _threadRenderer.Dispose();
            _threadRenderer = null;
        }

        internal void Update(Timing time)
        {
            EngineTask task = null;
            while (_taskQueue.TryDequeue(out task))
                task.Process(this, time);

            _input.Update(time);

            // Run through all the scenes and update if enabled.
            foreach(Scene scene in Scenes)
            { 
                if(scene.IsEnabled)
                    scene.Update(time);
            }
        }

        public void Dispose()
        {
            _log.WriteDebugLine("Shutting down engine");
            _threadManager.Dispose();
            _renderer?.Dispose();

            // Dispose of scenes
            for (int i = 0; i < Scenes.Count; i++)
                Scenes[i].Dispose();

            _log.Dispose();
            _settings.Save();
        }

        /// <summary>Gets the renderer attached to the current <see cref="Engine"/> instance.</summary>>
        public IRenderer Renderer => _renderer;

        /// <summary>Gets the log attached to the current <see cref="Engine"/> instance.</summary>
        internal Logger Log => _log;

        /// <summary>Gets the engine settings.</summary>
        public EngineSettings Settings => _settings;

        /// <summary>Gets the thread manager bound to the engine.</summary>
        public ThreadManager Threading => _threadManager;

        public ContentManager Content => _content;

        /// <summary>Gets the input manager attached to the current <see cref="Engine"/> instance.</summary>
        public InputManager Input => _input;
    }
}