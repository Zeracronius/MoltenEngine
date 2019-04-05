using SharpDX.D3DCompiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Molten.Graphics
{
    internal class HlslIncludeHandler : Include
    {
        MemoryStream _stream;
        IDisposable _disposable;
        ShaderEntryPoint _ep;

        internal HlslIncludeHandler(ShaderEntryPoint ep)
        {
            _ep = ep;
        }

        public void Close(Stream stream)
        {
            _stream.Close();
        }

        public Stream Open(IncludeType type, string className, Stream parentStream)
        {
            _stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(_stream); // Stream writer does not need disposing. Does not use any extra resources, but will close stream if disposed.
            writer.Write(_ep.Result.Includes[className].SourceCode);
            writer.Flush();
            _stream.Position = 0;
            return _stream;
        }

        public IDisposable Shadow
        {
            get => _disposable;
            set => _disposable = value;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
