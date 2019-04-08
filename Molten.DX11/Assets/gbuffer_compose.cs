using SharpShader;

namespace Molten.Assets
{
    public class GbufferComposeShader : CSharpShader
    {
        [Register(0)]
        public Texture2D<Vector4> mapLighting;

        [Register(1)]
        public Texture2D<Vector4> mapEmissive;

        public static Vector2 g_SpecPowerRange = new Vector2(10.0f, 250.0f);

        public SpriteCommonShader _common;

        [Semantic(SemanticType.SV_Target)]
        Vector4 PS_Compose(SpriteCommonShader.PS_IN input)
        {
            Vector4 col = _common.mapDiffuse.Sample(_common.diffuseSampler, input.uv);
            Vector4 light = mapLighting.Sample(_common.diffuseSampler, input.uv.XY);
            Vector4 emissive = mapEmissive.Sample(_common.diffuseSampler, input.uv.XY);

            return (col * light) + emissive;
        }
    }
}