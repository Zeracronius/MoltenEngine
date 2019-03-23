using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    [DataContract]
    public class MaterialDefinition
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<MaterialPassDefinition> Passes { get; } = new List<MaterialPassDefinition>();

        [DataMember]
        public ShaderBlendStateDefinition DefaultBlendState { get; set; } = ShaderBlendStateDefinition.Presets[BlendStatePreset.Default];

        [DataMember]
        public ShaderDepthStencilDefinition DepthStencilState { get; set; } = ShaderDepthStencilDefinition.Presets[DepthStencilPreset.Default];

        public ShaderRasterizerDefinition RasterizerState { get; set; } = ShaderRasterizerDefinition.Presets[RasterizerPreset.Default];
    }

    [DataContract]
    public class MaterialPassDefinition
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
        public ShaderBlendStateDefinition BlendState { get; set; }

        [DataMember]
        public ShaderDepthStencilDefinition DepthStencilState { get; set; }

        [DataMember]
        public ShaderRasterizerDefinition RasterizerState { get; set; }
    }
}
