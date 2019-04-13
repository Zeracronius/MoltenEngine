using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        static Dictionary<DepthStencilPreset, ShaderDepthStencilDefinition> _presets;

        public static ReadOnlyDictionary<DepthStencilPreset, ShaderDepthStencilDefinition> Presets { get; }

        static ShaderDepthStencilDefinition()
        {
            _presets = new Dictionary<DepthStencilPreset, ShaderDepthStencilDefinition>()
            {
                [DepthStencilPreset.Default] = new ShaderDepthStencilDefinition()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = ShaderDepthWriteMask.All,
                    DepthFunc = ComparisonMode.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 255,
                    StencilWriteMask = 255,
                },

                [DepthStencilPreset.DefaultNoStencil] = new ShaderDepthStencilDefinition()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = ShaderDepthWriteMask.All,
                    DepthFunc = ComparisonMode.Less,
                    IsStencilEnabled = false,
                    StencilReadMask = 255,
                    StencilWriteMask = 255,
                },

                [DepthStencilPreset.Sprite2D] = new ShaderDepthStencilDefinition()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = ShaderDepthWriteMask.All,
                    DepthFunc = ComparisonMode.LessEqual,
                    IsStencilEnabled = true,
                    StencilReadMask = 255,
                    StencilWriteMask = 255,
                },

                [DepthStencilPreset.ZDisabled] = new ShaderDepthStencilDefinition()
                {
                    IsDepthEnabled = false,
                    DepthWriteMask = ShaderDepthWriteMask.Zero,
                    DepthFunc = ComparisonMode.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 255,
                    StencilWriteMask = 255,
                },
            };

            Presets = new ReadOnlyDictionary<DepthStencilPreset, ShaderDepthStencilDefinition>(_presets);
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
        public ShaderDepthWriteMask DepthWriteMask { get; set; }

        [DataMember]
        public ComparisonMode DepthFunc { get; set; }

        [DataMember]
        public StateConditions Conditions { get; set; }

        [DataMember]
        public byte StencilReadMask { get; set; }

        [DataMember]
        public byte StencilWriteMask { get; set; }
    }
}
