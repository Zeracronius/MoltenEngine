﻿using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderComposition
    {
        /// <summary>A list of const buffers the shader stage requires to be bound.</summary>
        internal List<int> ConstBufferIds = new List<int>();

        /// <summary>A list of resources that must be bound to the shader stage.</summary>
        internal List<int> ResourceIds = new List<int>();

        /// <summary>A list of samplers that must be bound to the shader stage.</summary>
        internal List<int> SamplerIds = new List<int>();

        internal List<int> UnorderedAccessIds = new List<int>();

        internal ShaderIOStructure InputStructure;

        internal ShaderIOStructure OutputStructure;

        internal string EntryPoint;
    }

    internal class ShaderComposition<T> : ShaderComposition where T : DeviceChild
    {
        /// <summary>The underlying, compiled HLSL shader object.</summary>
        internal T RawShader;
    }
}
