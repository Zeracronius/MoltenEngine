using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    [DataContract]
    public class ShaderDefinition
    {
        public string FileName { get; internal set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<ShaderPassDefinition> Passes { get; } = new List<ShaderPassDefinition>();

        [DataMember]
        public ShaderBlendStateDefinition DefaultBlend { get; set; } = ShaderBlendStateDefinition.Presets[BlendStatePreset.Default];

        [DataMember]
        public ShaderDepthStencilDefinition DefaultDepthStencil { get; set; } = ShaderDepthStencilDefinition.Presets[DepthStencilPreset.Default];

        public ShaderRasterizerDefinition DefaultRasterizer { get; set; } = ShaderRasterizerDefinition.Presets[RasterizerPreset.Default];

        public List<string> Includes { get; set; } = new List<string>();
    }

    [DataContract]
    public class ShaderPassDefinition
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Iterations { get; set; }

        [DataMember]
        public string VertexEntryPoint { get; set; }

        [DataMember]
        public string FragmentEntryPoint { get; set; }

        [DataMember]
        public string GeometryEntryPoint { get; set; }

        [DataMember]
        public string HullEntryPoint { get; set; }

        [DataMember]
        public string DomainEntryPoint { get; set; }

        [DataMember]
        public string ComputeEntryPoint { get; set; }

        [DataMember]
        public ShaderBlendStateDefinition Blend { get; set; }

        [DataMember]
        public ShaderDepthStencilDefinition DepthStencil { get; set; }

        [DataMember]
        public ShaderRasterizerDefinition Rasterizer { get; set; }
    }
}
