﻿using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class MaterialLayoutValidator
    {
        internal bool Validate(ShaderCompilerContext<RendererDX11, HlslFoundation> context,
            MaterialPassCompileResult pResult)
        {
            bool valid = true;
            MaterialPass pass = pResult.Pass;
            ShaderComposition[] stages = pass.Compositions;
            ShaderComposition previous = null;

            // Stage order reference: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476882(v=vs.85).aspx
            previous = null;

            for (int i = 0; i < stages.Length; i++)
            {
                // Nothing to compare yet, continue.
                if (previous == null)
                {
                    previous = stages[i];
                    continue;
                }

                // No shader to compare. Go to next shader stage.
                if (string.IsNullOrWhiteSpace(stages[i].EntryPoint))
                    continue;

                ShaderIOStructure output = previous.OutputStructure;
                ShaderIOStructure input = stages[i].InputStructure;

                // If the input expects anything, check compatibility. Skip compat check if input does not expect anything (length 0).
                if (input.Data.Elements.Length > 0 && !output.IsCompatible(input))
                {
                    Type currentCompositionType = stages[i].GetType().GenericTypeArguments[0];
                    Type previousCompositionType = previous.GetType().GenericTypeArguments[0];

                    context.AddError("Incompatible material I/O structure.");
                    context.AddError("====================================");
                    context.AddError($"\tFilename: {pass.Material.Filename ?? "N/A"}");
                    context.AddError($"\tOutput -- {previousCompositionType.Name}:");

                    if (output.Data.Elements.Length > 0)
                    {
                        for (int o = 0; o < output.Data.Elements.Length; o++)
                            context.AddError($"\t\t[{o}] {output.Data.Names[o]} -- index: {output.Data.Elements[o].SemanticIndex}");
                    }
                    else
                    {
                        context.AddError("No output elements expected.");
                    }

                    context.AddError($"\tInput: {currentCompositionType.Name}:");
                    for (int o = 0; o < input.Data.Elements.Length; o++)
                        context.AddError($"\t\t[{o}] {input.Data.Names[o]} -- index: {input.Data.Elements[o].SemanticIndex}");

                    valid = false;
                }

                previous = stages[i];
            }

            return valid && 
                CheckTessellationShaders(context, pResult) && 
                CheckGeometryTessellationAdjacency(pResult);
        }

        private bool CheckTessellationShaders(
            ShaderCompilerContext<RendererDX11, HlslFoundation> context, 
            MaterialPassCompileResult pResult)
        {
            bool valid = true;
            FxcCompileResult hs = pResult.Results[MaterialPass.ID_HULL];
            FxcCompileResult ds = pResult.Results[MaterialPass.ID_DOMAIN];

            if(hs != null && ds == null)
            {
                context.AddError($"Material pass '{pResult.Pass.Name}' Has a hull shader but no domain shader. Both or neither must be present.");
                valid = false;
            }else if(hs == null && ds != null)
            {
                context.AddError($"Material pass '{pResult.Pass.Name}' Has a domain shader but no hull shader. Both or neither must be present.");
                valid = false;
            }

            return valid;
        }

        private bool CheckGeometryTessellationAdjacency(MaterialPassCompileResult pResult)
        {
            bool valid = true;
            FxcCompileResult geometryRef = pResult.Results[MaterialPass.ID_GEOMETRY];
            FxcCompileResult hullRef = pResult.Results[MaterialPass.ID_HULL];
            FxcCompileResult domainRef = pResult.Results[MaterialPass.ID_DOMAIN];

            if (geometryRef == null || hullRef == null || domainRef == null)
                return valid;

            /* Invalidate if tessellation is active with a geometry shader that expects adjacency data.
                * see: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476340%28v=vs.85%29.aspx
                * quote: "A geometry shader that expects primitives with adjacency (for example, 6 vertices per triangle) is 
                * not valid when tessellation is active (this results in undefined behavior, which the debug layer will complain about)."*/
            valid = pResult.Pass.GeometryPrimitive == D3DPrimitive.D3DPrimitiveLineAdj || 
                pResult.Pass.GeometryPrimitive == D3DPrimitive.D3DPrimitiveTriangleAdj;

            return valid;
        }
    }
}
