﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using ID3D10Blob = Silk.NET.Core.Native.ID3D10Blob;

namespace Molten.Graphics
{
    /// <summary>A helper class that safely wraps InputLayout.</summary>
    internal unsafe class VertexInputLayout : PipeBindable<ID3D11InputLayout>
    {
        ID3D11InputLayout* _native;
        bool _isValid = true;
        bool _isInstanced = false;
        ulong[] _expectedFormatIDs;

        internal VertexInputLayout(Device device, 
            ContextSlotGroup<BufferSegment> vbSlots, 
            ID3D10Blob* vertexBytecode,
            ShaderIOStructure io) : base(device, ContextBindTypeFlags.Input)
        {
            _expectedFormatIDs = new ulong[vbSlots.SlotCount];
            List<InputElementDesc> elements = new List<InputElementDesc>();
            VertexFormat format = null;

            // Store the EOID of each expected vertext format.
            for (uint i = 0; i < vbSlots.SlotCount; i++)
            {
                if (vbSlots[i].BoundValue == null)
                    continue;

                format = vbSlots[i].BoundValue.VertexFormat;

                /* Check if the current vertex segment's format matches 
                   the part of the shader's input structure that it's meant to represent. */
                int startID = elements.Count;
                if (!io.IsCompatible(format, (uint)startID))
                {
                    _isValid = false;
                    break;
                }

                // Collate vertex format elements into layout and set the correct input slot for each element.
                elements.AddRange(format.Data.Elements);

                for (int eID = startID; eID < elements.Count; eID++)
                {
                    InputElementDesc e = elements[eID];
                    e.InputSlot = i; // Vertex buffer input slot.
                    elements[eID] = e;

                    _isInstanced = _isInstanced || e.InputSlotClass == InputClassification.InputPerInstanceData;
                }

                _expectedFormatIDs[i] = format.EOID;
            }

            // Check if there are actually any elements. If not, use the default placeholder vertex type.
            if (elements.Count == 0)
            {
                VertexFormat nullFormat = device.VertexFormatCache.Get<VertexWithID>();
                elements.Add(nullFormat.Data.Elements[0]);
            }

            InputElementDesc[] finalElements = elements.ToArray();

            // Attempt creation of input layout.
            if (_isValid)
            {
                void* ptrByteCode = vertexBytecode->GetBufferPointer();
                nuint numBytes = vertexBytecode->GetBufferSize();
                device.NativeDevice->CreateInputLayout(ref finalElements[0], (uint)finalElements.Length,
                    ptrByteCode, numBytes, ref _native);
            }
            else
            {
                device.Log.Warning($"Vertex formats do not match the input layout of shader:");
                for (uint i = 0; i < vbSlots.SlotCount; i++)
                {
                    if (vbSlots[i].BoundValue == null)
                        continue;

                    format = vbSlots[i].BoundValue.VertexFormat;

                    device.Log.Warning("Format - Buffer slot " + i + ": ");
                    for (int f = 0; f < format.Data.Elements.Length; f++)
                        device.Log.Warning($"\t[{f}]{format.Data.Names[f]} -- index: {format.Data.Elements[f].SemanticIndex} -- slot: {i}");
                }

                // List final input structure.
                device.Log.Warning("Shader Input Structure: ");
                for (int i = 0; i < finalElements.Length; i++)
                    device.Log.Warning($"\t[{i}]{format.Data.Names[i]} -- index: {finalElements[i].SemanticIndex} -- slot: {finalElements[i].InputSlot}");
            }
        }

        internal override void Apply(DeviceContext pipe)
        {
            // Do nothing. Vertex input layouts build everything they need in the constructor.
        }

        public bool IsMatch(Logger log, ContextSlotGroup<BufferSegment> grp)
        {
            for (uint i = 0; i < grp.SlotCount; i++)
            {
                BufferSegment seg = grp[i].BoundValue;

                // If null vertex buffer, check if shader actually need one to be present.
                if (seg == null)
                {
                    // if shader's buffer hash is null for this slot, it's allowed to be null, otherwise no match.
                    if (_expectedFormatIDs[i] == 0)
                        continue;
                    else
                        return false;
                }

                // Prevent vertex buffer segments with no format from crashing the application.
                if (seg.VertexFormat == null)
                {
                    log.Warning($"Missing format for bound vertex segment {seg.Name} in slot {i}. Skipping validation. This may cause a false input layout match.");
                    continue;
                }

                if (seg.VertexFormat.EOID != _expectedFormatIDs[i])
                    return false;
            }

            return true;
        }

        internal override void PipelineRelease()
        {
            SilkUtil.ReleasePtr(ref _native);
        }

        /// <summary>Gets whether or not the input layout is valid.</summary>
        internal bool IsValid => _isValid;

        /// <summary>Gets whether or not the vertex input layout is designed for use with instanced draw calls.</summary>
        public bool IsInstanced => _isInstanced;

        internal override unsafe ID3D11InputLayout* NativePtr => _native;
    }
}
