using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Molten.Graphics
{
    internal class HlslCompiler : ShaderManagerBase
    {
#if RELEASE
         ShaderFlags _compileFlags = ShaderFlags.OptimizationLevel3;
#else
        ShaderFlags _compileFlags = ShaderFlags.WarningsAreErrors;
#endif

        internal static readonly string[] NewLineSeparators = new string[] { "\n", Environment.NewLine };
        RendererDX11 _renderer;

        Dictionary<string, ShaderNodeParser> _parsers;

        internal HlslCompiler(RendererDX11 renderer) : base(renderer, SharpShader.OutputLanguage.HLSL)
        {
            // Detect and instantiate node parsers
            _parsers = new Dictionary<string, ShaderNodeParser>();
            IEnumerable<Type> parserTypes = ReflectionHelper.FindTypeInParentAssembly<ShaderNodeParser>();
            foreach (Type t in parserTypes)
            {
                ShaderNodeParser parser = Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null) as ShaderNodeParser;
                foreach (string nodeName in parser.SupportedNodes)
                    _parsers.Add(nodeName, parser);
            }

            _renderer = renderer;
        }

        //internal IShader CompileEmbedded(string filename, Include includer = null)
        //{
        //    string source = null;
        //    using (Stream stream = EmbeddedResource.GetStream(filename, typeof(RendererDX11).Assembly))
        //    {
        //        using (StreamReader reader = new StreamReader(stream))
        //            source = reader.ReadToEnd();
        //    }

        //    return Compile(source, filename, includer);
        //}

        protected override void Preprocess(ShaderEntryPoint ep, Logger log)
        {
            try
            {
                Include includer = new HlslIncludeHandler(ep);
                ep.FinalSource = ShaderBytecode.Preprocess(ep.Result.SourceCode, null, includer, out ep.Error);
            }
            catch (Exception e)
            {
                ep.Error = e.Message;
            }
        }

        protected override IShader Compile(TranslatedShaderInfo info, Logger log)
        {
            ShaderCompilerContext context = new ShaderCompilerContext() { Compiler = this };

            // TODO replace the XML parsing section of the sub compilers with information from TranslatedShaderInfo.

            HlslShader shader = Parse(context, _renderer, info, log);
            foreach (HlslMessage msg in context.Messages)
            {
                if (msg.Type == HlslMessageType.Message)
                    log.WriteLine($"[SHADER][HLSL]{msg.Text}");
                else
                    log.WriteError($"[SHADER][HLSL]{msg.Text}");
            }

            return shader;
        }

        private HlslShader Parse(ShaderCompilerContext context, RendererDX11 renderer, TranslatedShaderInfo info, Logger log)
        {
            HlslShader shader = new HlslShader(renderer.Device, context.Filename);
            if (info.Passes.Count == 0)
            {
                shader.AddDefaultPass();
                if (string.IsNullOrWhiteSpace(shader.Passes[0].VertexShader.EntryPoint))
                {
                    context.AddError($"Material '{shader.Name}' does not have a defined vertex shader entry point. Must be defined in the material or it's first pass.");
                    return shader;
                }
            }

            // Parse and compile passes based on definition info
            foreach(TranslatedPassInfo pInfo in info.Passes)
            {
                HlslPass pass = new HlslPass(shader);
                ShaderPassCompileResult passResult = CompilePass(context, pass, log);

                foreach (ShaderBlendStateDefinition blendDef in pInfo.Definition.Blend)
                {
                    GraphicsBlendState state = new GraphicsBlendState(_renderer.Device, blendDef);
                    state = _renderer.Device.BlendBank.AddOrRetrieveExisting(state); // TODO can we just pass a definition straight in here? Avoid instantiating a state object?

                    if (blendDef.Conditions == StateConditions.None)
                        pass.BlendState.FillMissingWith(state);
                    else
                        pass.BlendState[blendDef.Conditions] = state;
                }

                foreach(ShaderDepthStencilDefinition depthDef in pInfo.Definition.DepthStencil)
                {
                    GraphicsDepthState state = new GraphicsDepthState(_renderer.Device, depthDef);
                    state = _renderer.Device.DepthBank.AddOrRetrieveExisting(state); // TODO same question as blend states here.

                    if (depthDef.Conditions == StateConditions.None)
                        pass.DepthState.FillMissingWith(state);
                    else
                        pass.DepthState[depthDef.Conditions] = state;
                }

                foreach(ShaderRasterizerDefinition rasterDef in pInfo.Definition.Rasterizer)
                {
                    GraphicsRasterizerState state = new GraphicsRasterizerState(_renderer.Device, rasterDef);
                    state = _renderer.Device.RasterizerBank.AddOrRetrieveExisting(state); // TODO same question as blend states here.

                    if (rasterDef.Conditions == StateConditions.None)
                        pass.RasterizerState.FillMissingWith(state);
                    else
                        pass.RasterizerState[rasterDef.Conditions] = state;
                }

                shader.AddPass(pass);
            }

            /* TODO:
            *  - State info will need to be parsed alongside shader entry-point info. This used to be done in ParseHeader().
            *  - Entry points will need compiling. We can still use HLSL reflection to confirm presence of various resources which may have been stripped during optimization/pre-process.
            *  - XML parsing can be replaced with TranslatedPassInfo.
            */

            //// Proceed to compiling each material pass.
            //ShaderPassCompileResult firstPassResult = null;
            //foreach (HlslPass pass in shader.Passes)
            //{
            //    ShaderPassCompileResult passResult = CompilePass(context, pass);
            //    firstPassResult = firstPassResult ?? passResult;

            //    if (context.HasErrors)
            //        return shader;
            //}

            // Validate the vertex input structure of all passes. Should match structure of first pass.
            // Only run this if there is more than 1 pass.
            if (shader.PassCount > 1)
            {
                ShaderIOStructure iStructure = shader.Passes[0].VertexShader.InputStructure;
                for (int i = 1; i < shader.PassCount; i++)
                {
                    if (!shader.Passes[i].VertexShader.InputStructure.IsCompatible(iStructure))
                        context.AddError($"Vertex input structure in Pass #{i + 1} in material '{shader.Name}' does not match structure of pass #1");
                }
            }

            // No issues arose, lets add it to the material manager
            if (!context.Messages.Any(m => m.Type == HlslMessageType.Error))
            {
                // Populate missing material states with default.
                shader.DepthState.FillMissingWith(renderer.Device.DepthBank.GetPreset(DepthStencilPreset.Default));
                shader.RasterizerState.FillMissingWith(renderer.Device.RasterizerBank.GetPreset(RasterizerPreset.Default));
                shader.BlendState.FillMissingWith(renderer.Device.BlendBank.GetPreset(BlendStatePreset.Default));

                ShaderSampler defaultSampler = renderer.Device.SamplerBank.GetPreset(SamplerPreset.Default);
                for (int i = 0; i < shader.Samplers.Length; i++)
                    shader.Samplers[i].FillMissingWith(defaultSampler);

                // First, attempt to populate pass states with their first conditional state. 
                // If that fails, fill remaining gaps with ones from material.
                foreach (HlslPass pass in shader.Passes)
                {
                    pass.DepthState.FillMissingWith(pass.DepthState[StateConditions.None]);
                    pass.DepthState.FillMissingWith(shader.DepthState);

                    pass.RasterizerState.FillMissingWith(pass.RasterizerState[StateConditions.None]);
                    pass.RasterizerState.FillMissingWith(shader.RasterizerState);

                    pass.BlendState.FillMissingWith(pass.BlendState[StateConditions.None]);
                    pass.BlendState.FillMissingWith(shader.BlendState);

                    // Ensure the pass can at least fit all of the base material samplers (if any).
                    if (pass.Samplers.Length < shader.Samplers.Length)
                    {
                        int oldLength = pass.Samplers.Length;
                        Array.Resize(ref pass.Samplers, shader.Samplers.Length);
                        for (int i = oldLength; i < pass.Samplers.Length; i++)
                            pass.Samplers[i] = new ShaderStateBank<ShaderSampler>();
                    }

                    for (int i = 0; i < pass.Samplers.Length; i++)
                    {
                        pass.Samplers[i].FillMissingWith(pass.Samplers[i][StateConditions.None]);

                        if (i >= shader.Samplers.Length)
                            pass.Samplers[i].FillMissingWith(defaultSampler);
                        else
                            pass.Samplers[i].FillMissingWith(shader.Samplers[i]);
                    }
                }

                shader.InputStructure = shader.Passes[0].VertexShader.InputStructure;
                shader.InputStructureByteCode = firstPassResult.VertexResult.Bytecode;

                shader.Scene = new SceneMaterialProperties(shader);
                shader.Object = new ObjectMaterialProperties(shader);
                shader.Textures = new GBufferTextureProperties(shader);
                shader.SpriteBatch = new SpriteBatchMaterialProperties(shader);
                shader.Light = new LightMaterialProperties(shader);
            }

            shader.DefaultResources = new IShaderResource[shader.Resources.Length];
            return shader;
        }

        private List<string> GetHeaders(string headerTagName, string source)
        {
            List<string> headers = new List<string>();

            Match m = Regex.Match(source, $"<{headerTagName}>(.|\n)*?</{headerTagName}>");
            while (m.Success)
            {
                headers.Add(m.Value);
                m = m.NextMatch();
            }

            return headers;
        }

        internal void ParseHeader(HlslFoundation foundation, ref string header, ShaderCompilerContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(header);

            XmlNode rootNode = doc.ChildNodes[0];
            ParseNode(foundation, rootNode, context);
        }

        internal void ParseNode(HlslFoundation foundation, XmlNode parentNode, ShaderCompilerContext context)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                string nodeName = node.Name.ToLower();
                ShaderNodeParser parser = null;
                if (_parsers.TryGetValue(nodeName, out parser))
                {
                    parser.Parse(foundation, context, node);
                }
                else
                {
                    if (parentNode.ParentNode != null)
                        context.AddMessage($"Ignoring unsupported {parentNode.ParentNode.Name} tag '{parentNode.Name}'");
                    else
                        context.AddMessage($"Ignoring unsupported root tag '{parentNode.Name}'");
                }
            }
        }

        protected bool HasResource(HlslShader shader, string resourceName)
        {
            foreach (ShaderResourceVariable resource in shader.Resources)
            {
                if (resource == null)
                    continue;

                if (resource.Name == resourceName)
                    return true;
            }

            return false;
        }

        protected bool HasConstantBuffer(ShaderCompilerContext context, HlslShader shader, string bufferName, string[] varNames)
        {
            foreach (ShaderConstantBuffer buffer in shader.ConstBuffers)
            {
                if (buffer == null)
                    continue;

                if (buffer.BufferName == bufferName)
                {
                    if (buffer.Variables.Length != varNames.Length)
                    {
                        context.AddError($"Material '{bufferName}' constant buffer does not have the correct number of variables ({varNames.Length})");
                        return false;
                    }

                    for (int i = 0; i < buffer.Variables.Length; i++)
                    {
                        ShaderConstantVariable variable = buffer.Variables[i];
                        string expectedName = varNames[i];

                        if (variable.Name != expectedName)
                        {
                            context.AddError($"Material '{bufferName}' constant variable #{i + 1} is incorrect: Named '{variable.Name}' instead of '{expectedName}'");
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        protected ShaderReflection BuildIO(CompilationResult code, ShaderComposition composition)
        {
            ShaderReflection shaderRef = new ShaderReflection(code);
            ShaderDescription desc = shaderRef.Description;
            composition.InputStructure = new ShaderIOStructure(shaderRef, ref desc, ShaderIOStructureType.Input);
            composition.OutputStructure = new ShaderIOStructure(shaderRef, ref desc, ShaderIOStructureType.Output);

            return shaderRef;
        }

        protected bool BuildStructure<T>(ShaderCompilerContext context, HlslShader shader, ShaderReflection shaderRef, CompilationResult code, ShaderComposition<T> composition)
            where T : DeviceChild
        {
            //build variable data
            ShaderDescription desc = shaderRef.Description;

            for (int r = 0; r < desc.BoundResources; r++)
            {
                InputBindingDescription binding = shaderRef.GetResourceBindingDescription(r);
                int bindPoint = binding.BindPoint;
                switch (binding.Type)
                {
                    case ShaderInputType.ConstantBuffer:
                        ConstantBuffer buffer = shaderRef.GetConstantBuffer(binding.Name);

                        // Skip binding info buffers
                        if (buffer.Description.Type != ConstantBufferType.ResourceBindInformation)
                        {
                            if (bindPoint >= shader.ConstBuffers.Length)
                                Array.Resize(ref shader.ConstBuffers, bindPoint + 1);

                            if (shader.ConstBuffers[bindPoint] != null && shader.ConstBuffers[bindPoint].BufferName != binding.Name)
                                context.AddMessage($"Material constant buffer '{shader.ConstBuffers[bindPoint].BufferName}' was overwritten by buffer '{binding.Name}' at the same register (b{bindPoint}).");

                            shader.ConstBuffers[bindPoint] = GetConstantBuffer(context, shader, buffer);
                            composition.ConstBufferIds.Add(bindPoint);
                        }

                        break;

                    case ShaderInputType.Texture:
                        OnBuildTextureVariable(context, shader, binding);
                        composition.ResourceIds.Add(binding.BindPoint);
                        break;

                    case ShaderInputType.Sampler:
                        bool isComparison = (binding.Flags & ShaderInputFlags.ComparisonSampler) == ShaderInputFlags.ComparisonSampler;
                        ShaderSamplerVariable sampler = GetVariableResource<ShaderSamplerVariable>(context, shader, binding);

                        if (bindPoint >= shader.SamplerVariables.Length)
                        {
                            int oldLength = shader.SamplerVariables.Length;
                            Array.Resize(ref shader.SamplerVariables, bindPoint + 1);
                            for (int i = oldLength; i < shader.SamplerVariables.Length; i++)
                                shader.SamplerVariables[i] = (i == bindPoint ? sampler : new ShaderSamplerVariable(shader));
                        }
                        else
                        {
                            shader.SamplerVariables[bindPoint] = sampler;
                        }
                        composition.SamplerIds.Add(bindPoint);
                        break;

                    case ShaderInputType.Structured:
                        BufferVariable bVar = GetVariableResource<BufferVariable>(context, shader, binding);
                        if (bindPoint >= shader.Resources.Length)
                            Array.Resize(ref shader.Resources, bindPoint + 1);

                        shader.Resources[bindPoint] = bVar;
                        composition.ResourceIds.Add(bindPoint);
                        break;

                    default:
                        OnBuildVariableStructure(context, shader, shaderRef, binding, binding.Type);
                        break;
                }

            }

            composition.RawShader = Activator.CreateInstance(typeof(T), shader.Device.D3d, code.Bytecode.Data, null) as T;
            return true;
        }

        protected void OnBuildVariableStructure(ShaderCompilerContext context, HlslShader shader, ShaderReflection reflection, InputBindingDescription binding, ShaderInputType inputType)
        {
            switch (inputType)
            {
                case ShaderInputType.UnorderedAccessViewRWStructured:
                        OnBuildRWStructuredVariable(context, shader, binding);
                    break;

                case ShaderInputType.UnorderedAccessViewRWTyped:
                        OnBuildRWTypedVariable(context, shader, binding);
                    break;
            }
        }

        private void OnBuildTextureVariable(ShaderCompilerContext context, HlslShader shader, InputBindingDescription binding)
        {
            ShaderResourceVariable obj = null;
            int bindPoint = binding.BindPoint;

            switch (binding.Dimension)
            {
                case ShaderResourceViewDimension.Texture1DArray:
                case ShaderResourceViewDimension.Texture1D:
                    obj = GetVariableResource<Texture1DVariable>(context, shader, binding);
                    break;

                case ShaderResourceViewDimension.Texture2DArray:
                case ShaderResourceViewDimension.Texture2D:
                    obj = GetVariableResource<Texture2DVariable>(context, shader, binding);
                    break;

                case ShaderResourceViewDimension.TextureCube:
                    obj = GetVariableResource<TextureCubeVariable>(context, shader, binding);
                    break;
            }

            if (bindPoint >= shader.Resources.Length)
                Array.Resize(ref shader.Resources, bindPoint + 1);

            //store the resource variable
            shader.Resources[bindPoint] = obj;
        }

        protected void OnBuildRWStructuredVariable(ShaderCompilerContext context, HlslShader shader, InputBindingDescription binding)
        {
            RWBufferVariable rwBuffer = GetVariableResource<RWBufferVariable>(context, shader, binding);
            int bindPoint = binding.BindPoint;

            if (bindPoint >= shader.UAVs.Length)
                Array.Resize(ref shader.UAVs, bindPoint + 1);

            shader.UAVs[bindPoint] = rwBuffer;
        }

        protected void OnBuildRWTypedVariable(ShaderCompilerContext context, HlslShader shader, InputBindingDescription binding)
        {
            RWVariable resource = null;
            int bindPoint = binding.BindPoint;

            switch (binding.Dimension)
            {
                case ShaderResourceViewDimension.Texture1D:
                    resource = GetVariableResource<RWTexture1DVariable>(context, shader, binding);
                    break;

                case ShaderResourceViewDimension.Texture2D:
                    resource = GetVariableResource<RWTexture2DVariable>(context, shader, binding);
                    break;
            }

            if (bindPoint >= shader.UAVs.Length)
                Array.Resize(ref shader.UAVs, bindPoint + 1);

            // Store the resource variable
            shader.UAVs[bindPoint] = resource;
        }

        private ShaderConstantBuffer GetConstantBuffer(ShaderCompilerContext context, HlslShader shader, ConstantBuffer buffer)
        {
            ShaderConstantBuffer cBuffer = new ShaderConstantBuffer(shader.Device, BufferMode.DynamicDiscard, buffer);
            string localName = cBuffer.BufferName;

            if (cBuffer.BufferName == "$Globals")
                localName += $"_{shader.Name}";

            // Duplication checks.
            if (context.ConstantBuffers.TryGetValue(localName, out ShaderConstantBuffer existing))
            {
                // Check for duplicates
                if (existing != null)
                {
                    // Compare buffers. If identical, 
                    if (existing.Hash == cBuffer.Hash)
                    {
                        // Dispose of new buffer, use existing.
                        cBuffer.Dispose();
                        cBuffer = existing;
                    }
                    else
                    {
                        LogHlslMessage(context, string.Format("Constant buffers with the same name ('{0}') do not match. Differing layouts.", localName));
                    }
                }
                else
                {
                    LogHlslMessage(context, string.Format("Constant buffer creation failed. A resource with the name '{0}' already exists!", localName));
                }
            }
            else
            {
                // Register all of the new buffer's variables
                foreach (ShaderConstantVariable v in cBuffer.Variables)
                {
                    // Check for duplicate variables
                    if (shader.Variables.ContainsKey(v.Name))
                    {
                        LogHlslMessage(context, "Duplicate variable detected: " + v.Name);
                        continue;
                    }

                    shader.Variables.Add(v.Name, v);
                }

                // Register the new buffer
                context.ConstantBuffers.Add(localName, cBuffer);
            }

            return cBuffer;
        }

        protected T GetVariableResource<T>(ShaderCompilerContext context, HlslShader shader, InputBindingDescription desc) where T : class, IShaderValue
        {
            IShaderValue existing = null;
            T bVar = null;
            Type t = typeof(T);

            if (shader.Variables.TryGetValue(desc.Name, out existing))
            {
                T other = existing as T;

                if (other != null)
                {
                    // If valid, use existing buffer variable.
                    if (other.GetType() == t)
                        bVar = other;
                }
                else
                {
                    LogHlslMessage(context, string.Format("Resource '{0}' creation failed. A resource with the name '{1}' already exists!", t.Name, desc.Name));
                }
            }
            else
            {
                bVar = Activator.CreateInstance(typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { shader }, null) as T;
                bVar.Name = desc.Name;

                shader.Variables.Add(bVar.Name, bVar);
            }

            return bVar;
        }

        /// <summary>Compiles HLSL source code and outputs the result. Returns true if successful, or false if there were errors.</summary>
        /// <param name="log"></param>
        /// <param name="entryPoint"></param>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool Compile(string entryPoint, ShaderType type, ShaderCompilerContext context, out CompilationResult result)
        {
            // Since it's not possible to have two functions in the same file with the same name, we'll just check if
            // a shader with the same entry-point name is already loaded in the context.
            if (!context.HlslShaders.TryGetValue(entryPoint, out result))
            {
                string strProfile = ShaderModel.Model5_0.ToProfile(type);
                result = ShaderBytecode.Compile(context.Source, entryPoint, strProfile, _compileFlags, EffectFlags.None, context.Filename);

                if (result.Message != null)
                {
                    LogHlslMessage(context, $"Material Pass ({entryPoint}) -- {result.Message}");
                    if (result.Message.Contains("error")) // NOTE: Workaround for SharpDX 4.0.1 where .HasErrors appears broken.
                        return false;
                }

                context.HlslShaders.Add(entryPoint, result);
            }

            return !result.HasErrors;
        }

        protected void LogHlslMessage(ShaderCompilerContext context, string txt)
        {
            string[] lines = txt.Split(HlslCompiler.NewLineSeparators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string msg = string.IsNullOrWhiteSpace(context.Filename) ? lines[i] : (context.Filename + ": " + lines[i]);
                if (lines[i].Contains("error"))
                    context.AddError(msg);
                else
                    context.AddMessage(msg);
            }
        }

        ShaderLayoutValidator _layoutValidator = new ShaderLayoutValidator();

        private ShaderPassCompileResult CompilePass(ShaderCompilerContext context, HlslPass pass, Logger log)
        {
            ShaderPassCompileResult result = new ShaderPassCompileResult(pass);

            // Compile each stage of the material pass.
            for (int i = 0; i < HlslPass.ShaderTypes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(pass.Compositions[i].EntryPoint))
                    continue;

                if (Compile(pass.Compositions[i].EntryPoint, HlslPass.ShaderTypes[i], context, out result.Results[i]))
                {
                    result.Reflections[i] = BuildIO(result.Results[i], pass.Compositions[i]);
                }
                else
                {
                    context.AddError($"{context.Filename}: Failed to compile {HlslPass.ShaderTypes[i]} stage of material pass.");
                    return result;
                }
            }

            // Fill in any extra metadata
            if (result.Reflections[(int)ShaderType.GeometryShader] != null)
                pass.GeometryPrimitive = result.Reflections[(int)ShaderType.GeometryShader].GeometryShaderSInputPrimitive;

            // Validate I/O structure of each shader stage.
            if (_layoutValidator.Validate(result, log))
                BuildPassStructure(context, result);

            return result;
        }

        private void BuildPassStructure(ShaderCompilerContext context, ShaderPassCompileResult pResult)
        {
            HlslPass pass = pResult.Pass;
            HlslShader shader = pass.Parent as HlslShader;
            DeviceDX11 device = shader.Device;

            // Vertex Shader
            if (pResult.VertexResult != null)
            {
                if (!BuildStructure(context, shader, pResult.VertexReflection, pResult.VertexResult, pass.VertexShader))
                    context.AddError($"Invalid vertex shader structure for '{pResult.Pass.VertexShader.EntryPoint}' in pass '{pResult.Pass.Name}'.");
            }

            // Hull Shader
            if (pResult.HullResult != null)
            {
                if (!BuildStructure(context, shader, pResult.HullReflection, pResult.HullResult, pass.HullShader))
                    context.AddError($"Invalid hull shader structure for '{pResult.Pass.HullShader.EntryPoint}' in pass '{pResult.Pass.Name}'.");
            }

            // Domain Shader
            if (pResult.DomainResult != null)
            {
                if (!BuildStructure(context, shader, pResult.DomainReflection, pResult.DomainResult, pass.DomainShader))
                    context.AddError($"Invalid domain shader structure for '{pResult.Pass.DomainShader.EntryPoint}' in pass '{pResult.Pass.Name}'.");
            }

            // Geometry Shader
            if (pResult.GeometryResult != null)
            {
                if (!BuildStructure(context, shader, pResult.GeometryReflection, pResult.GeometryResult, pass.GeometryShader))
                    context.AddError($"Invalid geometry shader structure for '{pResult.Pass.GeometryShader.EntryPoint}' in pass '{pResult.Pass.Name}'.");
            }

            // PixelShader Shader
            if (pResult.PixelResult != null)
            {
                if (!BuildStructure(context, shader, pResult.PixelReflection, pResult.PixelResult, pass.PixelShader))
                    context.AddError($"Invalid pixel shader structure for '{pResult.Pass.PixelShader.EntryPoint}' in pass '{pResult.Pass.Name}'.");
            }
        }
    }
}
