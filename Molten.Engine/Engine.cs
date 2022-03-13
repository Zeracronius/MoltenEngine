﻿using Molten.Collections;
using Molten.Font;
using Molten.Graphics;
using Molten.Input;
using Molten.Net;
using Molten.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Molten
{
    /// <summary>Stone Bolt engine. You only need one instance.</summary>
    public class Engine : IDisposable
    {
        ThreadedQueue<EngineTask> _taskQueue;
        List<EngineService> _services;
        EngineThread _mainThread;

        /// <summary>Gets the current instance of the engine. There can only be one active per application.</summary>
        public static Engine Current { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Engine"/>.
        /// </summary>
        /// <param name="initialSettings">The initial engine settings. If null, the default settings will be used instead.</param>
        /// <param name="ignoreSavedSettings">If true, the previously-saved settings will be ignored and replaced with the provided (or default) settings.</param>
        internal Engine(EngineSettings initialSettings, bool ignoreSavedSettings)
        {
            if (Current != null)
                throw new Exception("Cannot create more than one active instance of Engine. Dispose of the previous one first.");

            if (IntPtr.Size != 8)
                throw new NotSupportedException("Molten engine only supports 64-bit. Please build accordingly.");

            Current = this;
            Settings = initialSettings ?? new EngineSettings();

            if (!ignoreSavedSettings)
                Settings.Load();

            Log = Logger.Get();
            Log.AddOutput(new LogFileWriter("engine_log.txt"));
            Log.Debug("Engine Instantiated");
            Threading = new ThreadManager(Log);
            _taskQueue = new ThreadedQueue<EngineTask>();
            _services = new List<EngineService>(Settings.StartupServices);
            Content = new ContentManager(Log, this, Settings.ContentWorkerThreads);
            Scenes = new SceneManager(Settings.UI);

            Renderer = GetService<RenderService>();
            Input = GetService<InputService>();
            Net = GetService<NetworkService>();

            if (Renderer != null)
                Renderer.OnStarted += Renderer_OnStarted;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            foreach (EngineService service in _services)
                service.Initialize(Settings, Log);
        }

        private void Renderer_OnStarted(EngineService o)
        {
            LoadDefaultFont(Settings);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception);
            Logger.DisposeAll();
        }

        /// <summary>
        /// Retrieves an <see cref="EngineService"/> of the specified type. 
        /// Services are defined before an <see cref="Engine"/> instance is started.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T: EngineService
        {
            Type t = typeof(T);
            foreach(EngineService service in Settings.StartupServices)
            {
                Type serviceType = service.GetType();
                if (t.IsAssignableFrom(serviceType))
                    return service as T;
            }

            return null;
        }

        /// <summary>
        /// Gets whether a <see cref="EngineService"/> of the specified type is available. 
        /// </summary>
        /// <typeparam name="T">The type of engine service to check for availability.</typeparam>
        /// <returns></returns>
        public bool IsServiceAvailable<T>() where T : EngineService
        {
            return IsServiceAvailable(typeof(T));
        }

        /// <summary>
        /// Gets whether a <see cref="EngineService"/> of the specified type is available. 
        /// If a service state is not equal to <see cref="EngineServiceState.Running"/>, it is not considered as available.
        /// </summary>
        ///<param name="type">The <see cref="Type"/> of engine service to check for availability</param>
        /// <returns></returns>
        public bool IsServiceAvailable(Type type)
        {
            foreach (EngineService service in Settings.StartupServices)
            {
                Type serviceType = service.GetType();
                if (type.IsAssignableFrom(serviceType))
                    return service.State == EngineServiceState.Running || 
                        service.State == EngineServiceState.Ready;
            }

            return false;
        }

        private void LoadDefaultFont(EngineSettings settings)
        {
            try
            {
                using (FontReader reader = new FontReader(settings.DefaultFontName, Log))
                {
                    FontFile fontFile = reader.ReadFont(true);
                    DefaultFont = new SpriteFont(Renderer, fontFile, settings.DefaultFontSize);
                }
            }
            catch (Exception e)
            {
                // TODO Use the fallback font provided with the engine.
                Log.Error("Failed to load default font.");
                Log.Error(e);
                throw e;
            }
        }

        /// <summary>
        /// Starts the engine and it's services.
        /// </summary>
        public void Start(Action<Timing> updateCallback)
        {
            foreach (EngineService service in _services)
                service.Start(Threading, Log);

            Content.Workers.IsPaused = false;

            _mainThread = Threading.CreateThread("engine", true, true, (timing) =>
            {
                Update(timing);
                updateCallback(timing);
            });
        }

        /// <summary>
        /// Stops the engine and it's services.
        /// </summary>
        public void Stop()
        {
            _mainThread?.Dispose();

            foreach (EngineService service in _services)
            {
                service.Stop();

                // Wait for the service thread to stop.
                while (service.State != EngineServiceState.Ready)
                {
                    if (service.Thread != null)
                    {
                        if (service.Thread.IsDisposed)
                            break;
                        Thread.Sleep(10);
                    }
                }
            }
        }

        internal void AddScene(Scene scene)
        {
            EngineAddScene task = EngineAddScene.Get();
            task.Scene = scene;
            _taskQueue.Enqueue(task);
        }

        internal void RemoveScene(Scene scene)
        {
            EngineRemoveScene task = EngineRemoveScene.Get();
            task.Scene = scene;
            _taskQueue.Enqueue(task);
        }

        private void Update(Timing time)
        {
            while (_taskQueue.TryDequeue(out EngineTask task))
                task.Process(this, time);

            // Update services that are set to run on the main engine thread.
            foreach(EngineService service in _services)
            {
                if (service.ThreadMode == ThreadingMode.MainThread)
                    service.Update(time);
            }

            Scenes.Update(time);
        }

        /// <summary>
        /// Attempts to safely shutdown the engine before disposing of the current instance.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            Log.Debug("Disposing of engine");

            Stop();

            foreach (EngineService service in _services)
                service.Dispose();

            Threading.Dispose();
            Renderer?.Dispose();
            Scenes.Dispose();
            Content.Dispose();
            Logger.DisposeAll();
            Settings.Save();
            Current = null;
            IsDisposed = true;
        }

        /// <summary>
        /// [Internal] Gets the <see cref="Renderer"/> thread. Null if the renderer was not started.
        /// </summary>
        internal EngineThread RenderThread { get; private set; }

        /// <summary>Gets the <see cref="RenderService"/> attached to the current <see cref="Engine"/> instance.</summary>>
        public RenderService Renderer { get; private set; }

        /// <summary>Gets the <see cref="NetworkService"/> attached to the current <see cref="Engine"/> instance.</summary>>
        public NetworkService Net { get; private set; }

        /// <summary>Gets the log attached to the current <see cref="Engine"/> instance.</summary>
        internal Logger Log { get; }

        /// <summary>Gets the engine settings.</summary>
        public EngineSettings Settings { get; }

        /// <summary>Gets the thread manager bound to the engine.</summary>
        public ThreadManager Threading { get; }

        /// <summary>
        /// Gets the main <see cref="EngineThread"/> of the current <see cref="Engine"/> instance.
        /// Core/game update logic is usually done on this thread.
        /// </summary>
        public EngineThread MainThread { get; private set; }

        /// <summary>
        /// Gets the main content manager bound to the current engine instance. <para/>
        /// It is recommended that you use this to load assets that are unlikely to be unloaded throughout the lifetime of the current session. 
        /// You should use separate <see cref="ContentManager"/> instances for level-specific or short-lifespan content. 
        /// Disposing of a <see cref="ContentManager"/> instance will unload all of the content that was loaded by it.<para />
        /// </summary>
        public ContentManager Content { get; }

        /// <summary>
        /// Gets the default font as defined in <see cref="EngineSettings"/>.
        /// </summary>
        public SpriteFont DefaultFont { get; private set; }

        /// <summary>Gets the <see cref="InputService"/> attached to the current <see cref="Engine"/> instance.</summary>
        public InputService Input { get; private set; }

        /// <summary>
        /// Gets the internal scene manager for the current <see cref="Engine"/> instance.
        /// </summary>
        internal SceneManager Scenes { get; }

        /// <summary>
        /// Gets whether or not the current <see cref="Engine"/> instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
    }
}
