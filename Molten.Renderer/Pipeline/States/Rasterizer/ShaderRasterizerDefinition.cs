using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    [DataContract]
    public class ShaderRasterizerDefinition
    {
        [DataMember]
        public RasterizerCullMode CullMode { get; set; }

        [DataMember]
        public int DepthBias { get; set; }

        [DataMember]
        public float DepthBiasClamp { get; set; }

        [DataMember]
        public RasterizerFillMode FillMode { get; set; }

        [DataMember]
        public bool IsAntialiasedLineEnabled { get; set; }

        [DataMember]
        public bool IsDepthClipEnabled { get; set; }

        [DataMember]
        public bool IsFrontCounterClockwise { get; set; }

        [DataMember]
        public bool IsMultisampleEnabled { get; set; }

        [DataMember]
        public bool IsScissorEnabled { get; set; }

        [DataMember]
        public float SlopeScaledDepthBias { get; set; }
    }
}
