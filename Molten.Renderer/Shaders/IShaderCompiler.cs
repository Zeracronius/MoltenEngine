using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public interface IShaderCompiler
    {
        IShader Compile(TranslatedShaderInfo info);

        void Preprocess(ShaderEntryPoint ep);
    }
}
