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
    public class ShaderBlendStateDefinition
    {
        static Dictionary<BlendStatePreset, ShaderBlendStateDefinition> _presets;

        public static ReadOnlyDictionary<BlendStatePreset, ShaderBlendStateDefinition> Presets { get; }

        static ShaderBlendStateDefinition()
        {
            _presets = new Dictionary<BlendStatePreset, ShaderBlendStateDefinition>()
            {
                // Default values based on: https://docs.microsoft.com/en-us/windows/desktop/api/d3d11/ns-d3d11-d3d11_blend_desc
                [BlendStatePreset.Default] = new ShaderBlendStateDefinition(new ShaderBlendSlotDefinition()
                {
                    IsBlendEnabled = false,
                    SourceBlend = BlendFunc.One,
                    DestinationBlend = BlendFunc.Zero,
                    BlendOperation = BlendOp.Add,
                    SourceAlphaBlend = BlendFunc.One,
                    DestinationAlphaBlend = BlendFunc.Zero,
                    AlphaBlendOperation = BlendOp.Add,
                    RenderTargetWriteMask = BlendWriteMaskFlags.All,
                })
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false,
                },

                [BlendStatePreset.Additive] = new ShaderBlendStateDefinition(new ShaderBlendSlotDefinition()
                {
                    SourceBlend = BlendFunc.One,
                    DestinationBlend = BlendFunc.One,
                    BlendOperation = BlendOp.Add,
                    SourceAlphaBlend = BlendFunc.One,
                    DestinationAlphaBlend = BlendFunc.One,
                    AlphaBlendOperation = BlendOp.Add,
                    RenderTargetWriteMask = BlendWriteMaskFlags.All,
                    IsBlendEnabled = true,
                })
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false,
                },

                [BlendStatePreset.PreMultipliedAlpha] = new ShaderBlendStateDefinition(new ShaderBlendSlotDefinition()
                {
                    SourceBlend = BlendFunc.SourceAlpha,
                    DestinationBlend = BlendFunc.InverseSourceAlpha,
                    BlendOperation = BlendOp.Add,

                    SourceAlphaBlend = BlendFunc.InverseDestinationAlpha,
                    DestinationAlphaBlend = BlendFunc.One,
                    AlphaBlendOperation = BlendOp.Add,

                    RenderTargetWriteMask = BlendWriteMaskFlags.All,
                    IsBlendEnabled = true,
                })
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false,
                }
            };

            Presets = new ReadOnlyDictionary<BlendStatePreset, ShaderBlendStateDefinition>(_presets);
        }

        public ShaderBlendStateDefinition()
        {
            BlendFactor = new Color4(1, 1, 1, 1);
            BlendSampleMask = 0xffffffff;
        }

        public ShaderBlendStateDefinition(ShaderBlendSlotDefinition defaultSlotDefinition)
        {
            Targets.Add(defaultSlotDefinition);
            BlendFactor = new Color4(1, 1, 1, 1);
            BlendSampleMask = 0xffffffff;
        }

        [DataMember]
        public BlendStatePreset Preset { get; set; }

        [DataMember]
        public StateConditions Conditions { get; set; }

        [DataMember]
        public List<ShaderBlendSlotDefinition> Targets { get; } = new List<ShaderBlendSlotDefinition>();

        [DataMember]
        public bool AlphaToCoverageEnable { get; set; }

        [DataMember]
        public bool IndependentBlendEnable { get; set; }

        [DataMember]
        public Color4 BlendFactor { get; set; }

        [DataMember]
        public uint BlendSampleMask { get; set; }
    }

    [DataContract]
    public class ShaderBlendSlotDefinition
    {
        [DataMember]
        public BlendFunc SourceBlend { get; set; }

        [DataMember]
        public BlendFunc DestinationBlend { get; set; }

        [DataMember]
        public BlendOp BlendOperation { get; set; }

        [DataMember]
        public BlendFunc SourceAlphaBlend { get; set; }

        [DataMember]
        public BlendFunc DestinationAlphaBlend { get; set; }

        [DataMember]
        public BlendOp AlphaBlendOperation { get; set; }

        [DataMember]
        public BlendWriteMaskFlags RenderTargetWriteMask { get; set; }

        [DataMember]
        public bool IsBlendEnabled { get; set; } = true;
    }
}
