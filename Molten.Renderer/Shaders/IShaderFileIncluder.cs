using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public interface IShaderFileIncluder : IDisposable
    {
        void Close(Stream stream);
        Stream Open(string path);
    }

    public class ShaderFileIncluder : IShaderFileIncluder
    {
        Stream _stream;

        public void Close(Stream stream)
        {
            if(stream == _stream)
                _stream?.Close();
        }

        public Stream Open(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }

    public class ShaderEmbeddedIncluder : IShaderFileIncluder
    {
        Assembly _assembly;
        Stream _stream;
        string _namespace;

        /// <param name="assembly">The assembly from which embedded resource files will be loaded.</param>
        /// <param name="nSpace">The namespace from which the include handler will load files out of.</param>
        public ShaderEmbeddedIncluder(Assembly assembly, string nSpace)
        {
            _assembly = assembly;
        }

        public void Close(Stream stream)
        {
            if (stream == _stream)
                _stream?.Close();
        }

        public Stream Open(string path)
        {
            string embeddedName = _namespace + "." + path;
            return EmbeddedResource.GetStream(embeddedName, _assembly);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
