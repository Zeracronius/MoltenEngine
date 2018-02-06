﻿using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Molten.Graphics.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class PipelineInput : PipelineComponent
    {
        static VertexBufferBinding _nullVertexBuffer = new VertexBufferBinding(null, 0, 0);

        MaterialInputStage _materialStage;

        PipelineBindSlot<BufferSegment>[] _slotVertexBuffers;
        PipelineBindSlot<BufferSegment> _slotIndexBuffer;

        BufferSegment[] _vertexSegments;
        BufferSegment _indexSegment;
        VertexInputLayout _vertexLayout;
        MaterialPass _pass;

        int _vertexSlotCount = 0;

        PrimitiveTopology _currentTopology = PrimitiveTopology.TriangleList;
        List<VertexInputLayout> _cachedLayouts = new List<VertexInputLayout>();

        public PipelineInput(GraphicsPipe pipe) : base(pipe)
        {
            _materialStage = new MaterialInputStage(pipe);

            int vSlots = Device.Features.MaxVertexBufferSlots;
            _slotVertexBuffers = new PipelineBindSlot<BufferSegment>[vSlots];
            _vertexSegments = new BufferSegment[vSlots];

            for (int i = 0; i < vSlots; i++)
            {
                _slotVertexBuffers[i] = AddSlot<BufferSegment>(PipelineSlotType.Input, i);
                _slotVertexBuffers[i].OnBoundObjectDisposed += PipelineInput_OnBoundObjectDisposed;
            }

            int iSlots = Device.Features.MaxIndexBufferSlots;
            _slotIndexBuffer = new PipelineBindSlot<BufferSegment>(this, PipelineSlotType.Input, 0);
            _slotIndexBuffer.OnBoundObjectDisposed += _slotIndexBuffer_OnBoundObjectDisposed;
        }

        private void _slotIndexBuffer_OnBoundObjectDisposed(PipelineBindSlot slot, PipelineObject obj)
        {
            Pipe.Context.InputAssembler.SetIndexBuffer(null, SharpDX.DXGI.Format.Unknown, 0);
        }

        private void PipelineInput_OnBoundObjectDisposed(PipelineBindSlot slot, PipelineObject obj)
        {
            Pipe.Context.InputAssembler.SetVertexBuffers(slot.SlotID, _nullVertexBuffer);
        }

        internal override void Refresh()
        {
            // Update shader pipeline stages
            _materialStage.Refresh();

            // Update vertex buffers
            bool hasVBChanged = false;
            BufferSegment vb = null;
            for (int i = 0; i < _vertexSlotCount; i++)
            {
                vb = _vertexSegments[i];

                bool vbChanged = false;

                if (vb == null)
                    vbChanged = _slotVertexBuffers[i].Bind(Pipe, null);
                else
                    vbChanged = _slotVertexBuffers[i].Bind(Pipe, vb);

                // Check for change
                if (vbChanged)
                {
                    hasVBChanged = true;

                    if (vb == null)
                        Pipe.Context.InputAssembler.SetVertexBuffers(i, _nullVertexBuffer);
                    else
                        Pipe.Context.InputAssembler.SetVertexBuffers(i, vb.VertexBinding);
                }
            }

            Pipe.Context.InputAssembler.PrimitiveTopology = _currentTopology;

            // Update index buffers
            bool ibChanged = _slotIndexBuffer.Bind(Pipe, _indexSegment);
            if (ibChanged)
            {
                Pipe.Context.InputAssembler.SetIndexBuffer(
                    _indexSegment.Buffer,
                    _indexSegment.DataFormat,
                    _indexSegment.ByteOffset);
            }

            // Check if a change of layout is required
            if (hasVBChanged || _materialStage.HasMaterialChanged)
            {
                _vertexLayout = GetInputLayout();
                Pipe.Context.InputAssembler.InputLayout = _vertexLayout.Layout;
            }
        }

        /// <summary>Retrieves or creates a usable input layout for the provided vertex buffers and sub-effect.</summary>
        /// <returns>An instance of InputLayout.</returns>
        private VertexInputLayout GetInputLayout()
        {
            if (_materialStage.BoundShader == null)
                return null;

            // Retrieve layout list or create new one if needed.
            foreach(VertexInputLayout l in _cachedLayouts)
            {
                bool match = l.IsMatch(Device.Log, _slotVertexBuffers, _materialStage.BoundShader.InputStructure, _vertexSlotCount);
                if (match)
                    return l;
            }

            // A new layout is required
            VertexInputLayout input = new VertexInputLayout(Device, 
                _slotVertexBuffers, 
                _materialStage.BoundShader.InputStructureByteCode,
                _materialStage.BoundShader.InputStructure);
            _cachedLayouts.Add(input);

            return input;
        }

        internal GraphicsValidationResult Validate(GraphicsValidationMode mode)
        {
            GraphicsValidationResult result = GraphicsValidationResult.Successful;

            result |= CheckVertexSegment();

            // Validate and update mode-specific data if needed.
            switch (mode)
            {
                case GraphicsValidationMode.Indexed:
                    result |= CheckIndexSegment();
                    break;

                case GraphicsValidationMode.Instanced:
                    result |= CheckInstancing();
                    break;

                case GraphicsValidationMode.InstancedIndexed:
                    result |= CheckIndexSegment();
                    result |= CheckInstancing();
                    break;
            }

            result |= CheckMaterial();
            return result;
        }


        /// <summary>Validate vertex buffer and vertex shader.</summary>
        /// <param name="vbChanged">Has the vertex buffer changed.</param>
        /// <param name="veChanged">Has the vertex effect changed.</param>
        /// <returns></returns>
        private GraphicsValidationResult CheckMaterial()
        {
            GraphicsValidationResult result = GraphicsValidationResult.Successful;

            if (_materialStage.IsValid == false)
            {
                result |= GraphicsValidationResult.InvalidMaterial;
            }
            else
            {
                if (_materialStage.BoundShader == null)
                    result |= GraphicsValidationResult.MissingMaterial;
            }

            return result;
        }

        private GraphicsValidationResult CheckVertexSegment()
        {
            GraphicsValidationResult result = GraphicsValidationResult.Successful;

            if (_slotVertexBuffers[0].Object == null)
                result |= GraphicsValidationResult.MissingVertexSegment;

            return result;
        }

        private GraphicsValidationResult CheckIndexSegment()
        {
            GraphicsValidationResult result = GraphicsValidationResult.Successful;

            // If the index buffer is null, this method will always fail because 
            // it assumes it is only being called during an indexed draw call.
            if (_slotIndexBuffer.Object == null)
                result |= GraphicsValidationResult.MissingIndexSegment;

            return result;
        }

        private GraphicsValidationResult CheckInstancing()
        {
            if (_vertexLayout != null && _vertexLayout.IsInstanced)
                return GraphicsValidationResult.Successful;
            else
                return GraphicsValidationResult.NonInstancedVertexLayout;
        }

        internal void SetVertexSegment(BufferSegment seg, int slot)
        {
            if((seg.Parent.Description.BindFlags & BindFlags.VertexBuffer) != BindFlags.VertexBuffer)
                throw new InvalidOperationException("The provided buffer segment is not part of a vertex buffer.");

            EnsureVertexSlots(slot + 1);
            _vertexSegments[slot] = seg;
        }

        internal void SetVertexSegments(BufferSegment[] segments, int firstSlot, int startIndex, int segmentCount)
        {
            int end = startIndex + segmentCount;
            int slotID = firstSlot;
            EnsureVertexSlots(end);

            for (int i = startIndex; i < end; i++)
            {
                if (segments[i] != null)
                {
                    if ((segments[i].Parent.Description.BindFlags & BindFlags.VertexBuffer) != BindFlags.VertexBuffer)
                        throw new InvalidOperationException($"The provided buffer segment at index {i} is not part of a vertex buffer.");
                }

                _vertexSegments[slotID] = segments[i];
                slotID++;
            }
        }

        internal BufferSegment GetVertexSegment(int slot)
        {
            return _vertexSegments[slot];
        }

        internal void SetIndexSegment(BufferSegment seg)
        {
            if (seg != null)
            {
                if ((seg.Parent.Description.BindFlags & BindFlags.IndexBuffer) != BindFlags.IndexBuffer)
                    throw new InvalidOperationException("The provided buffer segment is not part of an index buffer.");
            }

            _indexSegment = seg;
        }

        internal void GetVertexSegments(BufferSegment[] destination)
        {
            for (int i = 0; i < _vertexSlotCount; i++)
                destination[i] = _vertexSegments[i];
        }

        internal BufferSegment GetIndexSegment()
        {
            return _indexSegment;
        }

        private void EnsureVertexSlots(int count)
        {
            _vertexSlotCount = _vertexSlotCount < count ? count : _vertexSlotCount;
        }

        protected override void OnDispose()
        {
            DisposeObject(ref _materialStage);

            // Dispose input layouts.
            for (int i = 0; i < _cachedLayouts.Count; i++)
                _cachedLayouts[i].Dispose();

            _cachedLayouts.Clear();

            base.OnDispose();
        }

        internal override bool IsValid { get { return true; } }

        internal Material Material
        {
            get => _materialStage.Shader;
            set => _materialStage.Shader = value;
        }

        /// <summary>Gets or sets the primitive topology.</summary>
        internal PrimitiveTopology Topology
        {
            get { return _currentTopology; }
            set { _currentTopology = value; }
        }

        internal int PassNumber
        {
            get => _materialStage.PassNumber;
            set => _materialStage.PassNumber = value;
        }
    }
}