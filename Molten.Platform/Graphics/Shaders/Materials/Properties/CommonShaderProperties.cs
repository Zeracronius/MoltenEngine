using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// A helper class for storing references to common shader properties, or filling missing ones in with dummy properties.
    /// </summary>
    public abstract class CommonShaderProperties
    {
        internal CommonShaderProperties(IShader shader) { }

        protected IShaderValue MapValue(IShader shader, string name)
        {
            return shader[name] ?? new DummyShaderValue();
        }
    }
}
