using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class GBufferTextureProperties : CommonShaderProperties
    {
        public IShaderValue DiffuseTexture { get; }

        public IShaderValue NormalTexture { get; }

        public IShaderValue EmissiveTexture { get; }

        public GBufferTextureProperties(IShader shader)  : base(shader)
        {
            DiffuseTexture = MapValue(shader, "mapDiffuse");
            NormalTexture = MapValue(shader, "mapNormal");
            EmissiveTexture = MapValue(shader, "mapEmissive");
        }
    }
}
