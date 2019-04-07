using SharpShader;

namespace Molten.Assets
{
    public class GBufferShader : CSharpShader
    {
        struct VS_IN
        {
            [Semantic(SemanticType.Position, 0)]
            public Vector4 pos;

            [Semantic(SemanticType.Normal, 0)]
            public Vector3 normal;

            [Semantic(SemanticType.Tangent, 0)]
            public Vector3 tangent;

            [Semantic(SemanticType.BiNormal, 0)]
            public Vector3 binormal;

            [Semantic(SemanticType.TexCoord, 0)]
            public Vector2 uv;
        }

        [ConstantBuffer, Register(0)]
        struct cbShadowGenVS
        {
            [PackOffset(0)]
            Matrix4x4 ShadowMat;
        }

        GBufferCommonShader _iCommon;
        GBufferCommonShader.Object _object;

        [VertexShader]
        GBufferCommonShader.VS_OUT VS(VS_IN input)
        {
            GBufferCommonShader.VS_OUT o;

            // NOTE ********* WVP should be calculated once per object on the CPU. NOT per vertex.
            Vector4 worldPos = Mul(input.pos, _object.world);
            Vector4 viewPos = Mul(worldPos, _iCommon._common.view);
            o.pos = Mul(viewPos, _iCommon._common.projection);

            o.normal = Mul(input.normal, (Matrix3x3)_object.world);
            o.normal = Normalize(o.normal);
            o.uv = input.uv;

            // Calculate the tangent vector against the world matrix only and then normalize the final value.
            o.tangent = Mul(input.tangent, (Matrix3x3)_object.world);
            o.tangent = Normalize(o.tangent);

            // Calculate the binormal vector against the world matrix only and then normalize the final value.
            o.binormal = Mul(input.binormal, (Matrix3x3)_object.world);
            o.binormal = Normalize(o.binormal);

            return o;
        }

        [FragmentShader]
        GBufferCommonShader.PS_OUT PS(GBufferCommonShader.VS_OUT input)
        {
            GBufferCommonShader.PS_OUT o = new GBufferCommonShader.PS_OUT();
            o.diffuse = _iCommon.mapDiffuse.Sample(_iCommon.texSampler, input.uv);            //output Color

            // Calculate normals
            Vector3 nMap = _iCommon.mapNormal.Sample(_iCommon.texSampler, input.uv).RGB;
            Vector3 glow = _iCommon.mapGlow.Sample(_iCommon.texSampler, input.uv).RGB;

            // Expand the range of the normal value from (0, +1) to (-1, +1).
            nMap = (nMap * 2.0f) - 1.0f;

            Vector3 normal = (nMap.X * input.tangent) + (nMap.y * input.binormal) + (nMap.z * input.normal);
            normal = Normalize(normal);

            o.normal.RGB = 0.5 * (normal + 1.0);
            o.emissive.RGB = glow * _object.emissivePower;

            // UNUSED
            // colorData.a
            // emissive.a
            o.emissive.A = 1;
            o.diffuse.A = 1;

            return o;
        }

        [FragmentShader]
        GBufferCommonShader.PS_OUT PS_Basic(GBufferCommonShader.VS_OUT input)
        {
            GBufferCommonShader.PS_OUT o = new GBufferCommonShader.PS_OUT();
            o.diffuse = _iCommon.mapDiffuse.Sample(_iCommon.texSampler, input.uv);
            o.emissive = _iCommon.mapGlow.Sample(_iCommon.texSampler, input.uv);
            o.normal.RGB = 0.5 * (input.normal + 1.0);

            // UNUSED
            // colorData.a
            // emissive.a
            // there is no normal.a because of 11,11,10 bit channels.
            o.emissive.A = 1;
            o.diffuse.A = 1;

            return o;
        }
    }
}