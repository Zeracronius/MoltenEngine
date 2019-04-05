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
    public class ShaderRasterizerDefinition
    {
        static Dictionary<RasterizerPreset, ShaderRasterizerDefinition> _presets;

        public static ReadOnlyDictionary<RasterizerPreset, ShaderRasterizerDefinition> Presets { get; }

        static ShaderRasterizerDefinition()
        {
            _presets = new Dictionary<RasterizerPreset, ShaderRasterizerDefinition>()
            {
                // Default values based on: https://docs.microsoft.com/en-us/windows/desktop/api/d3d11/ns-d3d11-d3d11_rasterizer_desc
                [RasterizerPreset.Default] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Solid,
                    CullMode = RasterizerCullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = false,
                    IsMultisampleEnabled = false,
                    IsAntialiasedLineEnabled = false,
                },

                [RasterizerPreset.ScissorTest] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Solid,
                    CullMode = RasterizerCullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = true,
                    IsMultisampleEnabled = false,
                    IsAntialiasedLineEnabled = false,
                },

                [RasterizerPreset.Multisample] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Solid,
                    CullMode = RasterizerCullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = false,
                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = false,
                },

                [RasterizerPreset.ScissorTestMultisample] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Solid,
                    CullMode = RasterizerCullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = true,
                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = false,
                },

                [RasterizerPreset.NoCulling] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Solid,
                    CullMode = RasterizerCullMode.None,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = false,
                    IsMultisampleEnabled = false,
                    IsAntialiasedLineEnabled = false,
                },

                [RasterizerPreset.Wireframe] = new ShaderRasterizerDefinition()
                {
                    FillMode = RasterizerFillMode.Wireframe,
                    CullMode = RasterizerCullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    IsScissorEnabled = false,
                    IsMultisampleEnabled = false,
                    IsAntialiasedLineEnabled = false,
                },
            };

            Presets = new ReadOnlyDictionary<RasterizerPreset, ShaderRasterizerDefinition>(_presets);
        }

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
