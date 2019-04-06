using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class ShaderInputStage : ShaderStage<HlslShader>
    {
        ShaderStep<VertexShader, VertexShaderStage, HlslShader> _vStage;
        ShaderStep<GeometryShader, GeometryShaderStage, HlslShader> _gStage;
        ShaderStep<HullShader, HullShaderStage, HlslShader> _hStage;
        ShaderStep<DomainShader, DomainShaderStage, HlslShader> _dStage;
        ShaderStep<PixelShader, PixelShaderStage, HlslShader> _pStage;

        int _passNumber = 0;
        bool _hasMaterialChanged;

        internal ShaderInputStage(PipeDX11 pipe) : base(pipe)
        {
            _vStage = CreateStep<VertexShader, VertexShaderStage>(pipe.Context.VertexShader, (stage, composition) => stage.Set(composition.RawShader));
            _gStage = CreateStep<GeometryShader, GeometryShaderStage>(pipe.Context.GeometryShader, (stage, composition) => stage.Set(composition.RawShader));
            _hStage = CreateStep<HullShader, HullShaderStage>(pipe.Context.HullShader, (stage, composition) => stage.Set(composition.RawShader));
            _dStage = CreateStep<DomainShader, DomainShaderStage>(pipe.Context.DomainShader, (stage, composition) => stage.Set(composition.RawShader));
            _pStage = CreateStep<PixelShader, PixelShaderStage>(pipe.Context.PixelShader, (stage, composition) => stage.Set(composition.RawShader));
        }

        private void _pStage_OnSetShader(HlslShader shader, ShaderComposition<PixelShader> composition, PixelShaderStage shaderStage)
        {
            shaderStage.Set(composition.RawShader);
        }

        private void _dStage_OnSetShader(HlslShader shader, ShaderComposition<DomainShader> composition, DomainShaderStage shaderStage)
        {
            shaderStage.Set(composition.RawShader);
        }

        private void _hStage_OnSetShader(HlslShader shader, ShaderComposition<HullShader> composition, HullShaderStage shaderStage)
        {
            shaderStage.Set(composition.RawShader);
        }

        private void _gStage_OnSetShader(HlslShader shader, ShaderComposition<GeometryShader> composition, GeometryShaderStage shaderStage)
        {
            shaderStage.Set(composition.RawShader);
        }

        private void _vStage_OnSetShader(HlslShader shader, ShaderComposition<VertexShader> composition, VertexShaderStage shaderStage)
        {
            shaderStage.Set(composition.RawShader);
        }

        internal void Refresh(HlslPass pass, StateConditions conditions)
        {
            // Reset pass number to 0 if the shader just changed.
            _hasMaterialChanged = _shader.Bind();

            if (_shader.BoundValue != null)
            {
                // Update samplers with those of the current pass
                int maxSamplers = Math.Min(_shader.BoundValue.SamplerVariables.Length, pass.Samplers.Length);
                for(int i = 0; i < maxSamplers; i++)
                    _shader.BoundValue.SamplerVariables[i].Value = pass.Samplers[i][conditions];

                // Refresh shader stages
                _vStage.Refresh(_shader.Value, pass.VertexShader);
                _gStage.Refresh(_shader.Value, pass.GeometryShader);
                _hStage.Refresh(_shader.Value, pass.HullShader);
                _dStage.Refresh(_shader.Value, pass.DomainShader);
                _pStage.Refresh(_shader.Value, pass.PixelShader);
            }
        }

        internal HlslPass CurrentPass => _shader.BoundValue?.Passes[_passNumber];

        /// <summary>Gets whether or not the material was changed during the last refresh call.</summary>
        internal bool HasMaterialChanged => _hasMaterialChanged;
    }
}
