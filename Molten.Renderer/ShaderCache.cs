using SharpShader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderCache : IDisposable
    {
        Translator _shaderTranslator;
        Dictionary<string, ShaderEntryPoint> _entryPointCache;
        char[] _pathSeparators = { '/' };
        OutputLanguage _language;
        MoltenRenderer _renderer;

        internal ShaderCache(MoltenRenderer renderer, OutputLanguage language)
        {
            _renderer = renderer;
            _language = language;
            _entryPointCache = new Dictionary<string, ShaderEntryPoint>();
            _shaderTranslator = new Translator("MoltenShaderCompiler");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epPath">The absolute path to the entry point. This is structured as "[file-path]/[class-name]/[entry-point-method]".</param>
        /// <param name="includes">A list of files to be included during the translation process, if no cached version is found.</param>
        /// <param name="log">A <see cref="Logger"/> for outputting any errors or messages which occur.</param>
        /// <param name="includer">An implementation of <see cref="IShaderFileIncluder"/> for loading and reading the content of a shader source file.</param>
        /// <returns></returns>
        internal ShaderEntryPoint GetShader(string epPath, List<string> includes, Logger log)
        {
            if (string.IsNullOrWhiteSpace(epPath))
                return null;

            string[] parts = epPath.Split(_pathSeparators, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length != 3)
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
                    foreach (KeyValuePair<string, EntryPointInfo> ep in result.Value.EntryPoints)
                    {
                        string entryPath = $"{result.Key}/{ep.Key}"; // {class path (i.e. namespace/class name}/{entry point name}
                        if (_entryPointCache.TryGetValue(entryPath, out epResult))
                            log.WriteError($"[SHADER] '{entryPath}' already existed when loading '{epPath}'. Using existing shader.");
                        else
                            _entryPointCache.Add(entryPath, new ShaderEntryPoint(result.Value, ep.Value));
                    }
                }
            }

            return epResult;
        }

        public void Dispose()
        {
            _shaderTranslator.Dispose();
        }
    }
}
