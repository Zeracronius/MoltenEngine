﻿using Molten.Threading;
using Molten.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public abstract class EngineService : EngineObject
    {
        public event MoltenEventHandler<EngineService> OnInitialized;

        /// <summary>
        /// Invoked after the current <see cref="EngineService"/> has 
        /// completed a <see cref="Start(ThreadManager, Logger)"/> invocation.
        /// </summary>
        public event MoltenEventHandler<EngineService> OnStarted;

        /// <summary>
        /// Invoked if an initialization or start-up error occurs.
        /// </summary>
        public event MoltenEventHandler<EngineService> OnError;

        LogFileWriter _logWriter;

        public EngineService()
        {
            Log = Logger.Get();
            string serviceName = this.GetType().Name;

            _logWriter = new LogFileWriter($"Logs/{serviceName}.txt");
            Log.AddOutput(_logWriter);
        }

        public void Initialize(EngineSettings settings, Logger parentLog)
        {
            Settings = settings;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                parentLog.Log($"Initializing service: {this.GetType()}");
                OnInitialize(Settings);
                parentLog.Log($"Completed initialization of service: {this}");
                State = EngineServiceState.Ready;

                sw.Stop();
                Log.Log($"Initialized service in {sw.Elapsed.TotalMilliseconds:N2} milliseconds");

                OnInitialized?.Invoke(this);
            }
            catch (Exception ex)
            {
                State = EngineServiceState.Error;
                parentLog.Error($"Failed to initialize service: {this}");
                parentLog.Error(ex);
            }
        }

        /// <summary>
        /// Requests the current <see cref="EngineService"/> to start.
        /// </summary>
        /// <param name="threadManager">The <see cref="ThreadManager"/> provided for startup.</param>
        /// <returns></returns>
        public void Start(ThreadManager threadManager, Logger parentLog)
        {
            if (State == EngineServiceState.Uninitialized)
                throw new EngineServiceException(this, "Cannot start uninitialized service.");

            if (State == EngineServiceState.Error)
            {
                parentLog.Error($"Cannot start service {this} due to error.");
                OnError?.Invoke(this);
                return;
            }

            if (State == EngineServiceState.Starting || State == EngineServiceState.Running)
                return;


            State = EngineServiceState.Starting;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ThreadMode = OnStart(threadManager);
                if (ThreadMode == ThreadingMode.SeparateThread)
                {
                    Thread = threadManager.CreateThread($"service_{this}", true, false, Update);
                    parentLog.Log($"Started service thread: {Thread.Name}");
                }

                State = EngineServiceState.Running;
                parentLog.Log($"Started service: {this}");
                parentLog.Log($"Service log: {_logWriter.LogFileInfo.FullName}");

                sw.Stop();
                Log.Log($"Started service in {sw.Elapsed.TotalMilliseconds:N2} milliseconds");

                OnStarted?.Invoke(this);
            }
            catch (Exception ex)
            {
                State = EngineServiceState.Error;
                parentLog.Error($"Failed to start service: {this}");
                parentLog.Error(ex);
                OnError?.Invoke(this);
            }
        }

        public void Stop()
        {
            OnStop();

            Thread?.Dispose();
            State = EngineServiceState.Ready;
        }

        protected override void OnDispose()
        {
            Thread?.DisposeAndJoin();

            State = EngineServiceState.Disposed;
            Thread = null;
            Log.Dispose();
        }

        public void Update(Timing time)
        {
            // TODO track update time taken.
            OnUpdate(time);
        }

        protected abstract void OnInitialize(EngineSettings settings);

        /// <summary>Invoked when the current <see cref="EngineService"/> needs to be updated.</summary>
        /// <param name="time"></param>
        protected abstract void OnUpdate(Timing time);

        /// <summary>
        /// Invokved when the current <see cref="EngineService"/> has been requested to start.
        /// </summary>
        /// <returns></returns>
        protected abstract ThreadingMode OnStart(ThreadManager threadManager);

        /// <summary>
        /// Invoked when the current <see cref="EngineService"/> has been requested to stop.
        /// </summary>
        protected abstract void OnStop();

        public EngineServiceState State { get; protected set; }

        /// <summary>
        /// Gets the thread bound to the current <see cref="EngineService"/>. 
        /// This will be null if <see cref="ThreadMode"/> is not set to <see cref="ThreadingMode.SeparateThread"/>.
        /// </summary>
        public EngineThread Thread { get; private set; }

        /// <summary>
        /// Gets the threading mode of the current <see cref="EngineService"/>.
        /// </summary>
        public ThreadingMode ThreadMode { get; protected set; }

        public EngineSettings Settings { get; private set; }

        /// <summary>
        /// Gets the log bound to the current <see cref="EngineService"/>.
        /// </summary>
        public Logger Log { get; private set; }
    }
}
