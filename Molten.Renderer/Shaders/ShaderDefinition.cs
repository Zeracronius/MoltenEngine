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
        public string Author { get; set; }


        [DataMember]
        public List<ShaderPassDefinition> Passes { get; set; } = new List<ShaderPassDefinition>();

        [DataMember]
        public List<ShaderBlendStateDefinition> DefaultBlend { get; set; } = new List<ShaderBlendStateDefinition>();

        [DataMember]
        public List<ShaderDepthStencilDefinition> DefaultDepthStencil { get; set; } = new List<ShaderDepthStencilDefinition>();

        [DataMember]
        public List<ShaderRasterizerDefinition> DefaultRasterizer { get; set; } = new List<ShaderRasterizerDefinition>();

        [DataMember]
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
        public List<ShaderBlendStateDefinition> Blend { get; set; }

        [DataMember]
        public List<ShaderDepthStencilDefinition> DepthStencil { get; set; }

        [DataMember]
        public List<ShaderRasterizerDefinition> Rasterizer { get; set; }
    }
}
