using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class HlslPass : HlslFoundation, IShaderPass
    {
        internal readonly static ShaderType[] ShaderTypes = new ShaderType[]
        {
            ShaderType.VertexShader,
            ShaderType.HullShader,
            ShaderType.DomainShader,
            ShaderType.GeometryShader,
            ShaderType.PixelShader,
            ShaderType.ComputeShader,
        };

        HlslShader _parent;

        internal HlslPass(HlslShader material) : base(material.Device)
        {
            _parent = material;

            VertexShader = new ShaderComposition<VertexShader>();
            HullShader = new ShaderComposition<HullShader>();
            DomainShader = new ShaderComposition<DomainShader>();
            GeometryShader = new ShaderComposition<GeometryShader>();
            PixelShader = new ShaderComposition<PixelShader>();
            ComputeShader = new ShaderComposition<ComputeShader>();
            Compositions = new ShaderComposition[ShaderTypes.Length];
            Compositions[(int)ShaderType.VertexShader] = VertexShader;
            Compositions[(int)ShaderType.HullShader] = HullShader;
            Compositions[(int)ShaderType.DomainShader] = DomainShader;
            Compositions[(int)ShaderType.GeometryShader] = GeometryShader;
            Compositions[(int)ShaderType.PixelShader] = PixelShader;
            Compositions[(int)ShaderType.ComputeShader] = ComputeShader;
        }

        internal GraphicsValidationResult ValidateInput(PrimitiveTopology topology)
        {
            GraphicsValidationResult result = GraphicsValidationResult.Successful;

            if(HullShader.RawShader != null)
            {
                if (topology < PrimitiveTopology.PatchListWith1ControlPoints)
                    result |= GraphicsValidationResult.HullPatchTopologyExpected;
            }

            return result;
        }

        internal ShaderComposition[] Compositions;

        internal ShaderComposition<VertexShader> VertexShader;

        internal ShaderComposition<HullShader> HullShader;

        internal ShaderComposition<DomainShader> DomainShader;

        internal ShaderComposition<GeometryShader> GeometryShader;

        internal ShaderComposition<PixelShader> PixelShader;

        internal ShaderComposition<ComputeShader> ComputeShader;

        internal InputPrimitive GeometryPrimitive;

        /// <summary>Gets or sets whether or not the pass will be run.</summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }

        public IShader Parent => _parent;

    }
}
