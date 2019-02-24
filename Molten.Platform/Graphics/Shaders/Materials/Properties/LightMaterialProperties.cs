using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class LightMaterialProperties : CommonShaderProperties
    {
        public IShaderValue Data { get;  }

        public IShaderValue MapDiffuse { get; }

        public IShaderValue MapNormal { get; }

        public IShaderValue MapDepth { get; }

        public IShaderValue InvViewProjection { get; }

        public IShaderValue CameraPosition { get; }

        public LightMaterialProperties(IMaterial material) : base(material)
        {
            Data = MapValue(material, "LightData");
            MapDiffuse = MapValue(material, "mapDiffuse");
            MapNormal = MapValue(material, "mapNormal");
            MapDepth = MapValue(material, "mapDepth");
            InvViewProjection = MapValue(material, "invViewProjection");
            CameraPosition = MapValue(material, "cameraPosition");
        }
    }
}
