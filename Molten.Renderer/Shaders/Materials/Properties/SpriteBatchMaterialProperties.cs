using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class SpriteBatchMaterialProperties : CommonShaderProperties
    {
        public IShaderValue TextureSize { get; }

        public SpriteBatchMaterialProperties(IShader shader) : base(shader)
        {
            TextureSize = MapValue(shader, "textureSize");
        }
    }
}
