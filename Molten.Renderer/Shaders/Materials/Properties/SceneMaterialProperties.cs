using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class SceneMaterialProperties : CommonShaderProperties
    {
        public IShaderValue View { get; private set; }

        public IShaderValue Projection { get; private set; }

        public IShaderValue ViewProjection { get; private set; }

        public IShaderValue InvViewProjection { get; private set; }

        public IShaderValue MaxSurfaceUV { get; private set; }

        public SceneMaterialProperties(IMaterial material) : base(material)
        {
            View = MapValue(material, "view");
            Projection = MapValue(material, "projection");
            ViewProjection = MapValue(material, "viewProjection");
            InvViewProjection = MapValue(material, "invViewProjection");
            MaxSurfaceUV = MapValue(material, "maxSurfaceUV");
        }
    }
}
