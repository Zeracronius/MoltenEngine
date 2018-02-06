﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Molten.Graphics
{
    internal class MaterialCompiler : HlslSubCompiler
    {
        // The names for expected constant buffers within each material pass.
        const string CONST_COMMON_NAME = "Common";
        const string CONST_OBJECT_NAME = "Object";

        static string[] CONST_COMMON_VAR_NAMES = new string[] { "view", "projection", "viewProjection", "invViewProjection" };
        static string[] CONST_OBJECT_VAR_NAMES = new string[] { "wvp", "world" };

        MaterialLayoutValidator _layoutValidator;

        internal MaterialCompiler(Logger log) : base(log)
        {
            AddParser<ShaderPassParser>("pass");

            _layoutValidator = new MaterialLayoutValidator();
        }

        internal override ShaderCompileResult Parse(RendererDX11 renderer, string header, string source, string filename = null)
        {
            ShaderCompileResult result = new ShaderCompileResult();
            Material material = new Material(renderer.Device, filename);
            try
            {
                ParseHeader(material, header);
            }
            catch (Exception e)
            {
                result.Errors.Add($"{filename ?? "Material header error"}: {e.Message}");
                return result;
            }

            // Proceed to compiling each material pass.
            MaterialPassCompileResult firstPassResult = null;
            foreach (MaterialPass pass in material.Passes)
            {
                MaterialPassCompileResult passResult = CompilePass(pass, source);
                firstPassResult = firstPassResult ?? passResult;

                result.Warnings.AddRange(passResult.Warnings);

                if (passResult.Errors.Count > 0)
                {
                    result.Errors.AddRange(passResult.Errors);
                    return result;
                }

                material.HasCommonConstants = material.HasCommonConstants || passResult.HasCommonConstants;
                material.HasObjectConstants = material.HasObjectConstants || passResult.HasObjectConstants;
            }

            // Validate the vertex input structure of all passes. Should match structure of first pass.
            // Only run this if there is more than 1 pass.
            if (material.PassCount > 1)
            {
                ShaderIOStructure iStructure = material.Passes[0].VertexShader.InputStructure;
                for (int i = 1; i < material.PassCount; i++)
                {
                    if (!material.Passes[i].VertexShader.InputStructure.IsCompatible(iStructure))
                    {
                        result.Errors.Add($"Vertex input structure in Pass #{i + 1} in material '{material.Name}' does not match structure of pass #1");
                        break;
                    }
                }
            }

            // No issues arose, lets add it to the material manager
            if (result.Errors.Count == 0) {
                material.InputStructure = material.Passes[0].VertexShader.InputStructure;
                material.InputStructureByteCode = firstPassResult.VertexResult.Bytecode;
                result.Shaders.Add(material);
                renderer.Materials.AddMaterial(material);
            }

            return result;
        }

        private MaterialPassCompileResult CompilePass(MaterialPass pass, string source)
        {
            string fn = pass.Material.Filename;
            MaterialPassCompileResult result = new MaterialPassCompileResult(pass);

            // Compile each stage of the material pass.
            for(int i = 0; i < MaterialPass.ShaderTypes.Length; i++)
            {
                if (pass.Compositions[i].Optional && string.IsNullOrWhiteSpace(pass.Compositions[i].EntryPoint))
                    continue;

                if (Compile(pass.Compositions[i].EntryPoint, MaterialPass.ShaderTypes[i], source, fn, out result.Results[i]))
                {
                    result.Reflections[i] = BuildIo(result.Results[i], pass.Compositions[i]);
                    bool hasCommonConstants = CheckForConstantBuffer(result, result.Reflections[i], CONST_COMMON_NAME, CONST_COMMON_VAR_NAMES);
                    bool hasObjectConstants = CheckForConstantBuffer(result, result.Reflections[i], CONST_OBJECT_NAME, CONST_OBJECT_VAR_NAMES);

                    result.HasCommonConstants = result.HasCommonConstants || hasCommonConstants;
                    result.HasObjectConstants = result.HasObjectConstants || hasObjectConstants;
                }
                else
                {
                    result.Errors.Add($"{fn}: Failed to compile {MaterialPass.ShaderTypes[i]} stage of material pass.");
                    return result;
                }
            }

            // Fill in any extra metadata
            if(result.Reflections[MaterialPass.ID_GEOMETRY] != null)
                pass.GeometryPrimitive = result.Reflections[MaterialPass.ID_GEOMETRY].GeometryShaderSInputPrimitive;

            // Validate I/O structure of each shader stage.
            if (_layoutValidator.Validate(result))
                BuildPassStructure(result);

            return result;
        }

        private bool CheckForConstantBuffer(MaterialPassCompileResult result, ShaderReflection reflection, string bufferName, string[] varNames)
        {
            ConstantBuffer buffer = reflection.GetConstantBuffer(bufferName);
            ConstantBufferDescription desc;
            try
            {
                desc = buffer.Description;
            }
            catch
            {
                return false;
            }

            // Validate layout of common buffer
            int varCount = desc.VariableCount;
            if (varCount != varNames.Length)
            {
                result.Errors.Add($"Material '{bufferName}' constant buffer does not have the correct number of variables ({varNames.Length})");
                return false;
            }

            for (int i = 0; i < varCount; i++)
            {
                ShaderReflectionVariable varDesc = buffer.GetVariable(i);
                ShaderReflectionType varType = varDesc.GetVariableType();
                ShaderTypeDescription typeDesc = varType.Description;

                string name = varDesc.Description.Name;
                string expectedName = varNames[i];
                if (name != expectedName)
                {
                    result.Errors.Add($"Material '{bufferName}' constant variable #{i + 1} is incorrect: Named '{name}' instead of '{expectedName}'");
                    return false;
                }

                if (typeDesc.Type != ShaderVariableType.Float || typeDesc.RowCount != 4 || typeDesc.ColumnCount != 4)
                {
                    result.Errors.Add($"Material '{bufferName}' constant variable #{i + 1}'s type is incorrect: '{typeDesc.Type.ToString().ToLower()}{typeDesc.RowCount}x{typeDesc.ColumnCount}' instead of 'float4x4'");
                    return false;
                }
            }

            return true;
        }

        private void BuildPassStructure(MaterialPassCompileResult pResult)
        {
            MaterialPass pass = pResult.Pass;
            Material material = pass.Material as Material;
            GraphicsDevice device = material.Device;

            // Vertex Shader
            if (pResult.VertexResult != null)
            {
                if (!BuildStructure(material, pResult.VertexReflection, pResult.VertexResult, pass.VertexShader))
                    pResult.Errors.Add("Invalid vertex shader structure.");
            }

            // Hull Shader
            if (pResult.HullResult != null)
            {
                if (!BuildStructure(material, pResult.HullReflection, pResult.HullResult, pass.HullShader))
                    pResult.Errors.Add("Invalid hull shader structure.");
            }

            // Domain Shader
            if (pResult.DomainResult != null)
            {
                if (!BuildStructure(material, pResult.DomainReflection, pResult.DomainResult, pass.DomainShader))
                    pResult.Errors.Add("Invalid domain shader structure.");
            }

            // Geometry Shader
            if (pResult.GeometryResult != null)
            {
                if (!BuildStructure(material, pResult.GeometryReflection, pResult.GeometryResult, pass.GeometryShader))
                    pResult.Errors.Add("Invalid geometry shader structure.");
            }

            // PixelShader Shader
            if (pResult.PixelResult != null)
            {
                if (!BuildStructure(material, pResult.PixelReflection, pResult.PixelResult, pass.PixelShader))
                    pResult.Errors.Add("Invalid pixel shader structure.");
            }
        }

        protected override void OnBuildVariableStructure(HlslShader shader, ShaderReflection reflection, InputBindingDescription binding, ShaderInputType inputType) { }
    }
}