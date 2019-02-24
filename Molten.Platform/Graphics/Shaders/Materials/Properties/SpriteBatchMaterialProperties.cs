using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class SpriteBatchMaterialProperties : CommonShaderProperties
    {
        public IShaderValue TextureSize { get; set; }

        public SpriteBatchMaterialProperties(IMaterial material) : base(material)
        {
            TextureSize = MapValue(material, "textureSize");
        }
    }
}
