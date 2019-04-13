using SharpShader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class TranslatedShaderInfo
    {
        public ShaderDefinition Definition { get; }

        public List<TranslatedPassInfo> Passes { get; set; } = new List<TranslatedPassInfo>();

        internal TranslatedShaderInfo(ShaderDefinition def)
        {
            Definition = def;
        }
    }
}
