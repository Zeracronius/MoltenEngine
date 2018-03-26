﻿using SharpDX.D3DCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderCompilerContext
    {
        internal ShaderCompileResult Result = new ShaderCompileResult();

        /// <summary>
        /// HLSL shader objects stored by entry-point name
        /// </summary>
        internal Dictionary<string, CompilationResult> HlslShaders = new Dictionary<string, CompilationResult>();

        internal List<GraphicsRasterizerState> RasterStates = new List<GraphicsRasterizerState>();

        internal List<GraphicsBlendState> BlendStates = new List<GraphicsBlendState>();

        internal List<GraphicsDepthState> DepthStates = new List<GraphicsDepthState>();

        internal List<ShaderSampler> Samplers = new List<ShaderSampler>();

        internal ShaderCompilerContext(HlslCompiler compiler)
        {
            Compiler = compiler;
        }

        internal HlslCompiler Compiler { get; private set; }

        internal string Filename;

        internal string Source;

        internal string Header;
    }
}
