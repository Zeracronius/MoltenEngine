using SharpDX.D3DCompiler;
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
        internal static readonly string[] NewLineSeparators = new string[] { "\n", Environment.NewLine };
        Dictionary<string, HlslSubCompiler> _subCompilers;
        Logger _log;
        RendererDX11 _renderer;

        Dictionary<string, ShaderNodeParser> _parsers;

        internal HlslCompiler(RendererDX11 renderer, Logger log) : base(renderer, SharpShader.OutputLanguage.HLSL)
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
            _log = log;
            _subCompilers = new Dictionary<string, HlslSubCompiler>();

            AddSubCompiler<MaterialCompiler>("material");
            AddSubCompiler<ComputeCompiler>("compute");
        }

        private void AddSubCompiler<T>(string nodeName) where T : HlslSubCompiler
        {
            Type t = typeof(T);
            HlslSubCompiler sub = Activator.CreateInstance(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,  null, null, null) as HlslSubCompiler;
            _subCompilers.Add(nodeName, sub);
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

        protected override string Preprocess(ShaderEntryPoint ep, Logger log)
        {
            string hlslError = null;
            try
            {
                Include includer = new HlslIncludeHandler(ep);
                ep.FinalSource = ShaderBytecode.Preprocess(ep.Result.SourceCode, null, includer, out hlslError);
            }
            catch (Exception e)
            {
                hlslError = e.Message;
            }
            return hlslError;
        }

        protected override IShader Compile(TranslatedShaderInfo info, Logger log)
        {
            ShaderCompilerContext context = new ShaderCompilerContext() { Compiler = this };
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

            // TODO replace the XML parsing section of the sub compilers with information from TranslatedShaderInfo.

            //// Pre-process HLSL source
            //string hlslError = "";
            //string finalSource = info;
            //try
            //{
            //    finalSource = ShaderBytecode.Preprocess(finalSource, null, includer ?? _defaultIncluder, out hlslError);
            //}
            //catch (Exception e)
            //{
            //    hlslError = e.Message;
            //}

            // Proceed if there is no pre-processor errors.
            if (!string.IsNullOrWhiteSpace(hlslError) == false)
            {
                context.Source = finalSource;
                context.Filename = filename;

                foreach (string nodeName in headers.Keys)
                {
                    HlslSubCompiler com = _subCompilers[nodeName];
                    List<string> nodeHeaders = headers[nodeName];
                    foreach (string header in nodeHeaders)
                    {
                        List<IShader> parseResult = com.Parse(context, _renderer, header);

                        // Intialize the shader's default resource array, now that we have the final count of the shader's actual resources.
                        foreach (HlslShader shader in parseResult)
                            shader.DefaultResources = new IShaderResource[shader.Resources.Length];

                        context.Result.AddResult(nodeName, parseResult);
                    }
                }
            }
            else
            {
                context.Errors.Add($"{filename ?? "Shader source error"}: {hlslError}");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                foreach (string error in context.Errors)
                    _log.WriteError(error);

                foreach (string msg in context.Messages)
                    _log.WriteLine(msg);
            }
            else
            {
                foreach (string error in context.Errors)
                    _log.WriteError($"{filename}: {error}");

                foreach (string msg in context.Messages)
                    _log.WriteLine($"{filename}: {msg}");
            }

            return context.Result;
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

        internal void ParserHeader(HlslFoundation foundation, ref string header, ShaderCompilerContext context)
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
                        context.Messages.Add($"Ignoring unsupported {parentNode.ParentNode.Name} tag '{parentNode.Name}'");
                    else
                        context.Messages.Add($"Ignoring unsupported root tag '{parentNode.Name}'");
                }
            }
        }
    }
}
