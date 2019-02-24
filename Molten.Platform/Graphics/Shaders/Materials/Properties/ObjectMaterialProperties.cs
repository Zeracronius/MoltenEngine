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

        public ObjectMaterialProperties(IMaterial material) : base(material)
        {
            World = MapValue(material, "world");
            Wvp = MapValue(material, "wvp");
            EmissivePower = MapValue(material, "emissivePower");
        }
    }
}
