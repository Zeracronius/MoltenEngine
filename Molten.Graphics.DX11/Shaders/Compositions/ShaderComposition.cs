﻿using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal abstract class ShaderComposition : ContextBindable
    {
        /// <summary>A list of const buffers the shader stage requires to be bound.</summary>
        internal List<uint> ConstBufferIds = new List<uint>();

        /// <summary>A list of resources that must be bound to the shader stage.</summary>
        internal List<uint> ResourceIds = new List<uint>();

        /// <summary>A list of samplers that must be bound to the shader stage.</summary>
        internal List<uint> SamplerIds = new List<uint>();

        internal List<uint> UnorderedAccessIds = new List<uint>();

        internal ShaderIOStructure InputStructure;

        internal ShaderIOStructure OutputStructure;

        internal string EntryPoint;

        internal ShaderType Type;

        internal bool Optional;

        internal HlslShader Parent { get; }

        internal unsafe abstract void SetBytecode(ID3D10Blob* byteCode);

        internal ShaderComposition(HlslShader parentShader, bool optional, ShaderType type) : 
            base(parentShader.Device, ContextBindTypeFlags.Input)
        {
            Parent = parentShader;
            Optional = optional;
            Type = type;
        }

        internal override void Apply(DeviceContext pipe) { }
    }

    internal abstract unsafe class ShaderComposition<T> : ShaderComposition 
        where T : unmanaged
    {
        T* _ptrShader;

        internal ShaderComposition(HlslShader parentShader, bool optional, ShaderType type) : 
            base(parentShader, optional, type) { }

        internal override unsafe void SetBytecode(ID3D10Blob* byteCode)
        {
            void* ptrBytecode = byteCode->GetBufferPointer();
            nuint numBytes = byteCode->GetBufferSize();
            _ptrShader = CreateShader(ptrBytecode, numBytes);
        }

        protected unsafe abstract T* CreateShader(void* ptrBytecode, nuint numBytes);

        /// <summary>The underlying, compiled HLSL shader object.</summary>
        internal T* PtrShader => _ptrShader;

        internal override void PipelineRelease()
        {
            SilkUtil.ReleasePtr(ref _ptrShader);
        }
    }
}
