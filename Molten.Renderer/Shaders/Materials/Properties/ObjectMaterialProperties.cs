using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class ObjectMaterialProperties : CommonShaderProperties
    {
        public IShaderValue World { get; }

        public IShaderValue Wvp { get; }

        public IShaderValue EmissivePower { get; }

        public ObjectMaterialProperties(IShader shader) : base(shader)
        {
            World = MapValue(shader, "world");
            Wvp = MapValue(shader, "wvp");
            EmissivePower = MapValue(shader, "emissivePower");
        }
    }
}
