using SharpDX.D3DCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderPassCompileResult
    {
        internal ShaderPassCompileResult(HlslPass pass)
        {
            Pass = pass;
            Results = new CompilationResult[HlslPass.ShaderTypes.Length];
            Reflections = new ShaderReflection[HlslPass.ShaderTypes.Length];
        }

        internal CompilationResult[] Results;

        internal ShaderReflection[] Reflections;

        internal ShaderIOStructure InputStructure;

        internal ShaderIOStructure OutputSructure;

        internal HlslPass Pass { get; private set; }

        internal CompilationResult VertexResult => Results[0];

        internal CompilationResult HullResult => Results[1];

        internal CompilationResult DomainResult => Results[2];

        internal CompilationResult GeometryResult => Results[3];

        internal CompilationResult PixelResult => Results[4];

        internal CompilationResult ComputeResult => Results[5];

        internal ShaderReflection VertexReflection => Reflections[0];

        internal ShaderReflection HullReflection => Reflections[1];

        internal ShaderReflection DomainReflection => Reflections[2];

        internal ShaderReflection GeometryReflection => Reflections[3];

        internal ShaderReflection PixelReflection => Reflections[4];

        internal ShaderReflection ComputeReflection => Reflections[5];
    }
}
