using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class SceneMaterialProperties : CommonShaderProperties
    {
        public IShaderValue View { get; }

        public IShaderValue Projection { get; }

        public IShaderValue ViewProjection { get; }

        public IShaderValue InvViewProjection { get; }

        public IShaderValue MaxSurfaceUV { get; }

        public SceneMaterialProperties(IShader shader) : base(shader)
        {
            View = MapValue(shader, "view");
            Projection = MapValue(shader, "projection");
            ViewProjection = MapValue(shader, "viewProjection");
            InvViewProjection = MapValue(shader, "invViewProjection");
            MaxSurfaceUV = MapValue(shader, "maxSurfaceUV");
        }
    }
}
