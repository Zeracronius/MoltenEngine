using SharpShader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public abstract class ShaderManagerBase
    {
        Translator _shaderTranslator;
        Dictionary<string, ShaderEntryPoint> _entryPointCache;
        char[] _pathSeparators = { '/' };
        OutputLanguage _language;
        MoltenRenderer _renderer;

        protected ShaderManagerBase(MoltenRenderer renderer, OutputLanguage language)
        {
            _renderer = renderer;
            _language = language;
            _entryPointCache = new Dictionary<string, ShaderEntryPoint>();
            _shaderTranslator = new Translator("MoltenShaderCompiler");
        }

        /// <summary>
        /// Compiles a shader using the current implementation of <see cref="MoltenRenderer"/>.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public IShader BuildShader(ShaderDefinition definition, Logger log)
        {
            /* TODO:
             *  - Content processor needs to populate ShaderDefinition.Filename
             *  - Content processor needs to convert file paths to absolute
             *  - Add Includes property to shader definition files. "includes": [ "file1", "file2" ]
             *  - Some definitions may use the same shader file (or even multiple times within the same definition)
             *      -- Store ShaderTranslationResult in ShaderCache
             *      -- Do not cache include files (unless they also contain shader entry points)
             *      -- This means translation can be skipped because the result is already known for at least some entry points of a shader definition
             *      
             *  - Populate MaterialDefinition or ComputeDefinition and forward to renderer implementation:
             *      -- BuildMaterial(MaterialDefinition definition) - Returns an IMaterial
             *      -- BuildCompute(ComputeDefinition definition) - Returns IComputeShader
             *      -- Distinguish a compute shader from a material simply by checking if compute entry point is populated
             *  
             *  - Implement better error reporting and validation
             */

            // If compute and other shader entry points are populated, we'll need to create both a material and a compute definition and forward them both individually.


            TranslatedShaderInfo matInfo = new TranslatedShaderInfo(definition);
            foreach (ShaderPassDefinition pDef in definition.Passes)
            {
                TranslatedPassInfo pInfo = new TranslatedPassInfo(pDef);
                pInfo.AddEntryPoint(EntryPointType.VertexShader, GetShader(pDef.VertexEntryPoint, definition.Includes, log));
                pInfo.AddEntryPoint(EntryPointType.FragmentShader, GetShader(pDef.FragmentEntryPoint, definition.Includes, log));
                pInfo.AddEntryPoint(EntryPointType.GeometryShader, GetShader(pDef.GeometryEntryPoint, definition.Includes, log));
                pInfo.AddEntryPoint(EntryPointType.HullShader, GetShader(pDef.HullEntryPoint, definition.Includes, log));
                pInfo.AddEntryPoint(EntryPointType.DomainShader, GetShader(pDef.DomainEntryPoint, definition.Includes, log));
                pInfo.AddEntryPoint(EntryPointType.ComputeShader, GetShader(pDef.ComputeEntryPoint, definition.Includes, log));
            }

            // TODO check if the shader is valid (e.g. material has at least vertex and pixel shader, or vertex and geometry when streaming, or hull + domain shaders).

            return Compile(matInfo, log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epPath">The absolute path to the entry point. This is structured as "[file-path]/[class-name]/[entry-point-method]".</param>
        /// <param name="includes">A list of files to be included during the translation process, if no cached version is found.</param>
        /// <param name="log">A <see cref="Logger"/> for outputting any errors or messages which occur.</param>
        /// <param name="includer">An implementation of <see cref="IShaderFileIncluder"/> for loading and reading the content of a shader source file.</param>
        /// <returns></returns>
        private ShaderEntryPoint GetShader(string epPath, List<string> includes, Logger log)
        {
            if (string.IsNullOrWhiteSpace(epPath))
                return null;

            string[] parts = epPath.Split(_pathSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                log.WriteError($"[SHADER] Invalid path '{epPath}': Does not contain at least 3 parts: file path, class name and entry-point name.");
                return null;
            }

            string className = parts[parts.Length - 2];
            string epName = parts[parts.Length - 1];
            string filePath = StringHelper.ConcatArray(parts, 0, parts.Length - 2);
            string epClassPath = $"{className}/{epName}";

            if (!_entryPointCache.TryGetValue(epClassPath, out ShaderEntryPoint epResult))
            {
                List<string> files = new List<string>();
                files.Add(filePath);
                files.AddRange(includes);

                Dictionary<string, string> sources = new Dictionary<string, string>();

                using (IShaderFileIncluder includer = new ShaderFileIncluder())
                {
                    foreach (string fn in files)
                    {
                        string fnAbsolute = Path.GetFullPath(fn);
                        Stream stream = includer.Open(fnAbsolute);
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string cSharpSource = reader.ReadToEnd();
                            sources.Add(fnAbsolute, cSharpSource);
                        }
                        includer.Close(stream);
                    }
                }

                TranslationResult tResult = _shaderTranslator.Translate(sources, _language);

                // Cache every translated shader class.
                foreach (KeyValuePair<string, ShaderTranslationResult> result in tResult)
                {
                    foreach (KeyValuePair<string, EntryPointInfo> entry in result.Value.EntryPoints)
                    {
                        string entryPath = $"{result.Key}/{entry.Key}"; // {class path (i.e. namespace/class name}/{entry point name}
                        if (_entryPointCache.TryGetValue(entryPath, out epResult))
                        {
                            log.WriteError($"[SHADER] '{entryPath}' already existed when loading '{epPath}'. Using existing shader.");
                        }
                        else
                        {
                            ShaderEntryPoint sep = new ShaderEntryPoint(result.Value, entry.Value);
                            _renderer.ShaderCompiler.Preprocess(sep, log);
                            _entryPointCache.Add(entryPath, sep);
                        }
                    }
                }
            }

            return epResult;
        }

        public void Dispose()
        {
            _shaderTranslator.Dispose();
        }

        protected abstract IShader Compile(TranslatedShaderInfo info, Logger log);

        protected abstract void Preprocess(ShaderEntryPoint ep, Logger log);
    }
}
