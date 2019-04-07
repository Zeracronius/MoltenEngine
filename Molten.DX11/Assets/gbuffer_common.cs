using SharpShader;

namespace Molten.Assets
{
    public class GBufferCommonShader : CSharpShader
    {
        public struct VS_OUT
        {
            [Semantic(SemanticType.SV_Position)]
            public Vector4 pos;

            [Semantic(SemanticType.TexCoord)]
            public Vector2 uv;

            [Semantic(SemanticType.Normal)]
            public Vector3 normal;

            [Semantic(SemanticType.Tangent)]
            public Vector3 tangent;

            [Semantic(SemanticType.BiNormal)]
            public Vector3 binormal;
        }

        //GBuffer Vertex shader output, without color
        public struct VS_OUTNOCOL
        {
            [Semantic(SemanticType.SV_Position)]
            Vector4 pos;

            [Semantic(SemanticType.TexCoord)]
            Vector2 uv;

            [Semantic(SemanticType.Normal)]
            Vector3 normal;

            [Semantic(SemanticType.Tangent)]
            Vector3 tangent;

            [Semantic(SemanticType.BiNormal)]
            Vector3 binormal;
        }

        public struct PS_OUT
        {
            [Semantic(SemanticType.SV_Target, 0)]
            public Vector4 diffuse;

            [Semantic(SemanticType.SV_Target, 1)]
            public Vector3 normal;

            [Semantic(SemanticType.SV_Target, 2)]
            public Vector4 emissive;
        };

        [ConstantBuffer, Register(1)]
        public struct Common
        {
            [PackOffset(0)]
            public Matrix4x4 view;

            [PackOffset(4)]
            public Matrix4x4 projection;

            [PackOffset(8)]
            public Matrix4x4 viewProjection;

            [PackOffset(12)]
            public Matrix4x4 invViewProjection;

            [PackOffset(16)]
            public Vector2 maxSurfaceUV;
        }

        [ConstantBuffer, Register(2)]
        public struct Object
        {
            [PackOffset(0)]
            public Matrix4x4 wvp;

            [PackOffset(4)]
            public Matrix4x4 world;

            [PackOffset(8)]
            public float emissivePower;
        }

        [Register(1)]
        public TextureSampler texSampler;

        [Register(0)]
        public Texture2D<Vector4> mapDiffuse;

        [Register(1)]
        public Texture2D<Vector3> mapNormal;

        [Register(2)]
        public Texture2D<Vector4> mapGlow;

        [Register(3)]
        public Texture2D<Vector4> mapSpecular;

        [Register(4)]
        public Texture2D<float> mapDepth;

        public static Vector2 g_SpecPowerRange = new Vector2( 10.0f, 250.0f );

        public struct MATERIAL
        {
            public Vector3 colorData;
            public Vector4 worldPos;
            public Vector3 normal;
            public Vector3 specular;
            public Vector2 uv;
            public float depth;
        }


        public GBufferCommonShader.Common _common;

        public MATERIAL UnpackGBuffer(Vector3 screenPos)
        {
            MATERIAL o = new MATERIAL();

            // Obtain screen position
            screenPos.XY /= screenPos.Z;

            // Obtain textureCoordinates corresponding to the current pixel
            // The screen coordinates are in [-1,1]*[1,-1]
            // The texture coordinates need to be in [0,1]*[0,1]
            o.uv = 0.5f * (new Vector2(screenPos.X, -screenPos.Y) + 1);
            o.uv *= _common.maxSurfaceUV;

            // Get color and specular
            Vector4 colorData = mapDiffuse.Sample(texSampler, o.uv);
            Vector3 normalData = mapNormal.Sample(texSampler, o.uv);
            Vector4 specData = mapSpecular.Sample(texSampler, o.uv);

            // Tranform normal back into [-1,1] range
            o.normal = 2.0f * normalData.XYZ - 1.0f;

            o.colorData = colorData.RGB;
            o.specular = specData.RGB;

            // UNUSED
            // colorData.a
            // emissive.a
            // there is no normal.a because of 11,11,10 bit channels.

            // Read depth
            o.depth = mapDepth.Sample(texSampler, o.uv).R;

            // Compute screen-space position
            o.worldPos.XY = screenPos.XY;
            o.worldPos.Z = o.depth;
            o.worldPos.W = 1.0f;

            // Transform to world space
            o.worldPos = Mul(o.worldPos, _common.invViewProjection);
            o.worldPos /= o.worldPos.W;

            return o;
        }
    }
}