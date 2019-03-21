using System;
using SharpShader;

namespace Molten.Samples.Assets
{
    public class BasicTextureShader : CSharpShader
    {
        struct VS_IN
        {
            [Semantic(SemanticType.Position)]
            public Vector4 pos;

            [Semantic(SemanticType.TexCoord)]
            public Vector2 uv;
        }

        struct PS_IN
        {
            [Semantic(SemanticType.Position)]
            public Vector4 pos;

            [Semantic(SemanticType.TexCoord)]
            public Vector2 uv;
        }

        [ConstantBuffer, Register(0)]
        struct CommonData
        {
            [PackOffset(0)]
            public Matrix4x4 view;

            [PackOffset(4)]
            public Matrix4x4 projection;

            [PackOffset(8)]
            public Matrix4x4 viewProjection;

            [PackOffset(12)]
            public Matrix4x4 invViewProjection;
        }

        [ConstantBuffer, Register(1)]
        struct ObjectData
        {
            [PackOffset(0)]
            public Matrix4x4 wvp;

            [PackOffset(4)]
            Matrix4x4 world;
        }

        Texture2D mapAlbedo;
        TextureSampler texSampler;

        ObjectData obj;

        PS_IN VS(VS_IN input)
        {
            PS_IN output = new PS_IN();
            output.pos = Mul(input.pos, obj.wvp);
            output.uv = input.uv;
            return output;
        }

        [Semantic(SemanticType.SV_Target)]
        Vector4 PS(PS_IN input)
        {
            return mapAlbedo.Sample(texSampler, input.uv);
        }
    }
}
