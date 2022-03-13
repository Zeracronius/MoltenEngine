﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class CSComposition : ShaderComposition<ID3D11ComputeShader>
    {
        public CSComposition(HlslShader parentShader, bool optional, ShaderType type) : 
            base(parentShader, optional, type)
        {
        }

        protected override unsafe ID3D11ComputeShader* CreateShader(void* ptrBytecode, nuint numBytes)
        {
            ID3D11ComputeShader* ppShader = null;
            Parent.Device.NativeDevice->CreateComputeShader(ptrBytecode, numBytes, null, &ppShader);
            return ppShader;
        }
    }
}
