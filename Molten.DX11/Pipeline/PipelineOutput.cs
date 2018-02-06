﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class PipelineOutput : PipelineComponent
    {
        enum SlotType
        {
            RenderSurface = 0,
            DepthBuffer = 1,
        }

        GraphicsPipe _pipe;
        PipelineBindSlot<RenderSurfaceBase>[] _slotSurfaces;
        PipelineBindSlot<DepthSurface> _slotDepth;

        RenderSurfaceBase[] _surfaces;
        DepthSurface _depthSurface = null;
        RenderTargetView[] _rtViews;
        DepthStencilView _depthView = null;

        GraphicsDepthMode _boundMode = GraphicsDepthMode.Enabled;
        GraphicsDepthMode _depthMode = GraphicsDepthMode.Enabled;

        public PipelineOutput(GraphicsPipe pipe) : base(pipe.Device)
        {
            _pipe = pipe;

            int maxRTs = Device.Features.SimultaneousRenderSurfaces;
            _slotSurfaces = new PipelineBindSlot<RenderSurfaceBase>[maxRTs];
            _surfaces = new RenderSurfaceBase[maxRTs];
            _rtViews = new RenderTargetView[maxRTs];

            for (int i = 0; i < maxRTs; i++)
            {
                _slotSurfaces[i] = AddSlot<RenderSurfaceBase>(PipelineSlotType.Output, i);
                _slotSurfaces[i].OnBoundObjectDisposed += SurfaceSlot_OnBoundObjectDisposed;
            }

            _slotDepth = AddSlot<DepthSurface>(PipelineSlotType.Output, 0);
            _slotDepth.OnBoundObjectDisposed += _slotDepth_OnBoundObjectDisposed;
        }

        private void SurfaceSlot_OnBoundObjectDisposed(PipelineBindSlot slot, PipelineObject obj)
        {
            _rtViews[slot.SlotID] = null;
            _pipe.Context.OutputMerger.SetTargets(_depthView, _rtViews);
            Pipe.Profiler.RenderTargetSwaps++;
        }

        private void _slotDepth_OnBoundObjectDisposed(PipelineBindSlot slot, PipelineObject obj)
        {
            _depthView = null;
            _pipe.Context.OutputMerger.SetTargets(_depthView, _rtViews);
            Pipe.Profiler.RenderTargetSwaps++;
        }

        internal override void Refresh()
        {
            bool rtChangeDetected = false;

            // Check for render surface changes
            RenderTargetView rtv = null;
            for (int i = 0; i < _surfaces.Length; i++)
            {
                bool rtChanged = _slotSurfaces[i].Bind(_pipe, _surfaces[i]);
                rtv = _slotSurfaces[i].BoundObject != null ? _slotSurfaces[i].BoundObject.RTV : null;

                if (rtChanged || rtv != _rtViews[i])
                {
                    rtChangeDetected = true;

                    if (_slotSurfaces[i].BoundObject == null)
                        _rtViews[i] = null;
                    else
                        _rtViews[i] = _slotSurfaces[i].BoundObject.RTV;
                }
            }

            // Check depth surface for changes
            bool depthChanged = _slotDepth.Bind(_pipe, _depthSurface);
            if (_slotDepth.BoundObject == null)
            {
                _depthView = null;
            }
            else
            {
                DepthStencilView oldDepthView = _depthView;
                GraphicsDepthMode oldDepthMode = _depthMode;

                _boundMode = _depthMode;

                switch (_depthMode)
                {
                    case GraphicsDepthMode.Disabled:
                        _depthView = null;
                        break;

                    case GraphicsDepthMode.Enabled:
                        _depthView = _slotDepth.BoundObject.DepthView;
                        break;

                    case GraphicsDepthMode.ReadOnly:
                        _depthView = _slotDepth.BoundObject.ReadOnlyView;
                        break;
                }

                depthChanged = _depthMode != oldDepthMode || _depthView != oldDepthView;
            }

            // Check if changes need to be forwarded to the GPU.
            if (rtChangeDetected || depthChanged)
            {
                _pipe.Context.OutputMerger.SetTargets(_depthView, _rtViews);
                Pipe.Profiler.RenderTargetSwaps++;
            }
        }

        public void SetDepthSurface(DepthSurface surface, GraphicsDepthMode depthMode)
        {
            _depthSurface = surface;
            _depthMode = depthMode;
        }

        public DepthSurface GetDepthSurface()
        {
            return _depthSurface;
        }

        public void SetDepthMode(GraphicsDepthMode value)
        {
            _depthMode = value;
        }

        public GraphicsDepthMode GetDepthMode()
        {
            return _depthMode;
        }

        /// <summary>Sets a list of render surfaces.</summary>
        /// <param name="surfaces">The surfaces.</param>
        /// <param name="count">The number of surfaces to set.</param>
        public void SetRenderSurfaces(RenderSurfaceBase[] surfaces, int count)
        {
            for (int i = 0; i < count; i++)
                _surfaces[i] = surfaces[i];

            // Set the remaining surfaces to null.
            for (int i = count; i < _surfaces.Length; i++)
                _surfaces[i] = null;

            // Ensure target 0 is reset back to the device output if needed.
            if (_surfaces[0] == null)
                _surfaces[0] = Device.DefaultSurface;
        }

        /// <summary>
        /// Sets the render surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="slot">The slot.</param>
        public void SetRenderSurface(RenderSurfaceBase surface, int slot)
        {
            if (surface == null)
            {
                if (slot == 0)
                    _surfaces[slot] = Device.DefaultSurface;
                else
                    _surfaces[slot] = null;
            }
            else
            {
                _surfaces[slot] = surface;
            }
        }

        /// <summary>
        /// Fills the provided array with a list of applied render surfaces.
        /// </summary>
        /// <param name="destinationArray">The array to fill with applied render surfaces.</param>
        public void GetRenderSurfaces(RenderSurfaceBase[] destinationArray)
        {
            for (int i = 0; i < _surfaces.Length; i++)
                destinationArray[i] = _surfaces[i];
        }

        /// <summary>Gets the render surface located in the specified output slot.</summary>
        /// <param name="slot">The ID of the slot to retrieve from.</param>
        /// <returns></returns>
        public RenderSurfaceBase GetRenderSurface(int slot)
        {
            return _surfaces[slot];
        }

        /// <summary>Resets the render surface contained in an output slot.</summary>
        /// <param name="resetMode"></param>
        /// <param name="slot"></param>
        public void ResetRenderSurface(RenderSurfaceResetMode resetMode, int slot)
        {
            switch (resetMode)
            {
                case RenderSurfaceResetMode.OutputSurface:
                    _surfaces[slot] = Device.DefaultSurface;
                    break;

                case RenderSurfaceResetMode.NullSurface:
                    _surfaces[slot] = null;
                    break;
            }

        }

        /// <summary>
        /// Resets the render surfaces.
        /// </summary>
        /// <param name="resetMode">The reset mode.</param>
        /// <param name="outputOnFirst">If true, and the reset mode is OutputSurface, it will only be applied to the first slot (0)..</param>
        public void ResetRenderSurfaces(RenderSurfaceResetMode resetMode, bool outputOnFirst = true)
        {
            switch (resetMode)
            {
                case RenderSurfaceResetMode.OutputSurface:
                    if (outputOnFirst)
                    {
                        for (int i = 0; i < _surfaces.Length; i++)
                            _surfaces[i] = null;

                        _surfaces[0] = Device.DefaultSurface;
                    }
                    else
                    {
                        for (int i = 0; i < _surfaces.Length; i++)
                            _surfaces[i] = Device.DefaultSurface;
                    }
                    break;

                case RenderSurfaceResetMode.NullSurface:
                    for (int i = 0; i < _surfaces.Length; i++)
                        _surfaces[i] = null;
                    break;
            }
        }

        /// <summary>Clears a render target that is set on the device.</summary>
        /// <param name="color"></param>
        /// <param name="slot"></param>
        public void Clear(Color color, int slot)
        {
            if (_surfaces[slot] != null)
                _surfaces[slot].Clear(_pipe, color);
        }

        internal GraphicsValidationResult Validate()
        {
            // TODO if render target 0 is set, ensure a pixel shader is bound, otherwise flag as missing pixel shader.

            return GraphicsValidationResult.Successful;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        internal override bool IsValid { get { return true; } }

        /// <summary>
        /// Gets whether or not a render target has been applied at slot 0.
        /// </summary>
        internal bool TargetZeroSet { get { return _rtViews[0] != null; } }

        /// <summary>
        /// Gets the <see cref="RenderSurfaceBase"/> at the specified slot.
        /// </summary>
        public RenderSurfaceBase this[int slotIndex]
        {
            get { return _surfaces[slotIndex]; }
        }
    }
}