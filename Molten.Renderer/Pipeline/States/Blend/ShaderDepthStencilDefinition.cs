using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    [DataContract]
    public class ShaderDepthStencilDefinition
    {
        [DataContract]
        public class Face
        {
            [DataMember]
            public ComparisonMode Comparison { get; set; }

            [DataMember]
            public StencilOp PassOperation { get; set; }

            [DataMember]
            public StencilOp FailOperation { get; set; }

            [DataMember]
            public StencilOp DepthFailOperation { get; set; }
        }

        [DataMember]
        public DepthStencilPreset Preset { get; set; }

        [DataMember]
        public int StencilReference { get; set; }

        [DataMember]
        public GraphicsDepthWritePermission WritePermission { get; set; }

        /// <summary>Gets the description for the front-face depth operation description.</summary>
        [DataMember]
        public Face FrontFace { get; } = new Face();

        /// <summary>Gets the description for the back-face depth operation description.</summary>
        [DataMember]
        public Face BackFace { get; } = new Face();

        [DataMember]
        public bool IsDepthEnabled { get; set; }

        [DataMember]
        public bool IsStencilEnabled { get; set; }

        [DataMember]
        /// <summary>
        /// Gets or sets the depth write mask. True to allow writing depth changes, false to write zero.
        /// </summary>
        public bool DepthWriteMaskAll { get; set; }

        [DataMember]
        public ComparisonMode DepthComparison { get; set; }

        [DataMember]
        public byte StencilReadMask { get; set; }

        [DataMember]
        public byte StencilWriteMask { get; set; }
    }
}
