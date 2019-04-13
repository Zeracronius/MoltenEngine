using SharpDX.D3DCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderCompilerContext
    {
        /// <summary>
        /// HLSL shader objects stored by entry-point name
        /// </summary>
        internal Dictionary<string, CompilationResult> HlslShaders = new Dictionary<string, CompilationResult>();

        internal Dictionary<string, ShaderConstantBuffer> ConstantBuffers = new Dictionary<string, ShaderConstantBuffer>();

        internal List<HlslMessage> Messages = new List<HlslMessage>();

        internal HlslCompiler Compiler;

        internal string Filename;

        internal string Source;

        internal void AddMessage(string msg)
        {
            Messages.Add(new HlslMessage(msg, HlslMessageType.Message));
        }

        internal void AddError(string msg)
        {
            Messages.Add(new HlslMessage(msg, HlslMessageType.Error));
            HasErrors = true;
        }

        internal void AddWarning(string msg)
        {
            Messages.Add(new HlslMessage(msg, HlslMessageType.Warning));
        }

        internal bool HasErrors { get; private set; }
    }

    internal class HlslMessage
    {
        internal string Text;

        internal HlslMessageType Type;

        internal HlslMessage(string text, HlslMessageType type)
        {
            Text = text;
            Type = type;
        }
    }

    internal enum HlslMessageType
    {
        Message = 0,

        Error = 1,

        Warning = 2,
    }
}
