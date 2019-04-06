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

        public LightMaterialProperties(IShader shader) : base(shader)
        {
            Data = MapValue(shader, "LightData");
            MapDiffuse = MapValue(shader, "mapDiffuse");
            MapNormal = MapValue(shader, "mapNormal");
            MapDepth = MapValue(shader, "mapDepth");
            InvViewProjection = MapValue(shader, "invViewProjection");
            CameraPosition = MapValue(shader, "cameraPosition");
        }
    }
}
