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
    public class ShaderSamplerDefinition
    {
        static Dictionary<SamplerPreset, ShaderSamplerDefinition> _presets;
        public static ReadOnlyDictionary<SamplerPreset, ShaderSamplerDefinition> Presets { get; }

        static ShaderSamplerDefinition()
        {
            _presets = new Dictionary<SamplerPreset, ShaderSamplerDefinition>()
            {
                [SamplerPreset.Default] = new ShaderSamplerDefinition()
                {
                    AddressU = SamplerAddressMode.Wrap,
                    AddressV = SamplerAddressMode.Wrap,
                    AddressW = SamplerAddressMode.Wrap,
                }
            };
        }

        /// <summary>Gets or sets the method to use for resolving a U texture coordinate that is outside the 0 to 1 range.</summary>
        [DataMember]
        public SamplerAddressMode AddressU { get; set; }

        /// <summary>Gets or sets the method to use for resolving a V texture coordinate that is outside the 0 to 1 range.</summary>
        [DataMember]
        public SamplerAddressMode AddressV { get; set; }

        /// <summary>Gets or sets the method to use for resolving a W texture coordinate that is outside the 0 to 1 range.</summary>
        [DataMember]
        public SamplerAddressMode AddressW { get; set; }

        /// <summary>Border color to use if SharpDX.Direct3D11.TextureAddressMode.Border is specified 
        /// for AddressU, AddressV, or AddressW. Range must be between 0.0 and 1.0 inclusive.</summary>
        [DataMember]
        public Color4 BorderColor { get; set; }

        /// <summary>A function that compares sampled data against existing sampled data. 
        /// The function options are listed in SharpDX.Direct3D11.Comparison.</summary>
        [DataMember]
        public ComparisonMode ComparisonFunc { get; set; }

        /// <summary>Gets or sets the filtering method to use when sampling a texture (see SharpDX.Direct3D11.Filter).</summary>
        [DataMember]
        public SamplerFilter Filter { get; set; }

        /// <summary>Clamping value used if SharpDX.Direct3D11.Filter.Anisotropic or SharpDX.Direct3D11.Filter.ComparisonAnisotropic 
        /// is specified in SamplerFilter. Valid values are between 1 and 16.</summary>
        [DataMember]
        public int MaxAnisotropy { get; set; }

        /// <summary>Upper end of the mipmap range to clamp access to, where 0 is the largest
        ///     and most detailed mipmap level and any level higher than that is less detailed.
        ///     This value must be greater than or equal to MinLOD. To have no upper limit
        ///     on LOD set this to a large value such as D3D11_FLOAT32_MAX.</summary>
        [DataMember]
        public float MaxMipMapLod { get; set; }

        /// <summary>Lower end of the mipmap range to clamp access to, where 0 is the largest and most detailed mipmap level 
        /// and any level higher than that is less detailed.</summary>
        [DataMember]
        public float MinMipMapLod { get; set; }

        /// <summary>Gets or sets the offset from the calculated mipmap level. For example, if Direct3D calculates 
        /// that a texture should be sampled at mipmap level 3 and MipLODBias is 2, then 
        /// the texture will be sampled at mipmap level 5.</summary>
        [DataMember]
        public float LodBias { get; set; }

        /// <summary>Gets whether or not the sampler a comparison sampler. This is determined by the <see cref="Filter"/> mode.</summary>
        [DataMember]
        public bool IsComparisonSampler { get; }
    }
}
