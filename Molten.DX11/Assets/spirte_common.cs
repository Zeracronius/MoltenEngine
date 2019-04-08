using SharpShader;

namespace Molten.Assets
{
    public class SpriteCommonShader : CSharpShader
    {
        struct VS_GS
        {
            [Semantic(SemanticType.Position, 0)]
            public Vector2 pos;

            [Semantic(SemanticType.Position, 1)]
            public Vector2 size;


            [Semantic(SemanticType.Position, 2)]
            public Vector2 origin;


            [Semantic(SemanticType.Position, 3)]
            public Vector4 uv;

            [Semantic(SemanticType.Position, 4)]
            public float rotation;

            [Semantic(SemanticType.Position, 5)]
            public float arraySlice;

            [Semantic(SemanticType.Color)]
            public Vector4 col;
        }

        public struct PS_IN
        {
            [Semantic(SemanticType.SV_Position)]
            public Vector4 pos;

            [Semantic(SemanticType.Color)]
            public Vector4 col;

            [Semantic(SemanticType.TexCoord)]
            public Vector3 uv;
        }

        public Texture2DArray mapDiffuse;
        public TextureSampler diffuseSampler;

        Matrix4x4 wvp;
        Vector2 textureSize;

        static float degToRad360 = 6.28319f;

        VS_GS VS(VS_GS input)
        {
            input.uv.XZ /= textureSize.X;
            input.uv.YW /= textureSize.Y;

            //invert y axis.
            input.pos.Y = -input.pos.Y;

            // Invert Y origin
            input.origin.Y = -input.origin.Y;

            return input;
        }

        VS_GS VS_Line(VS_GS input)
        {
            //invert y axis.
            input.pos.Y = -input.pos.Y;
            input.size.Y = -input.size.Y; // y position of 2nd line point.
            return input;
        }

        VS_GS VS_Circle(VS_GS input)
        {
            //invert y axis.
            input.pos.Y = -input.pos.Y;
            return input;
        }

        VS_GS VS_Tri(VS_GS input)
        {
            //invert y axis.
            input.pos.Y = -input.pos.Y;
            input.size.Y = -input.size.Y;
            input.origin.Y = -input.origin.Y;
            return input;
        }

        static Vector2[] spriteCorners = {
            new Vector2(0,-1),
            new Vector2(0, 0),
            new Vector2(1,-1),
            new Vector2(1,0),
        };

        static Int2[] uvTable = {
            new Int2(0,3),
            new Int2(0,1),
            new Int2(2,3),
            new Int2(2,1),
        };

        Matrix2x2 GetRotation(float angle)
        {
            // Compute a 2x2 rotation matrix.
            float c = Cos(angle);
            float s = Sin(angle);

            return new Matrix2x2(c, -s, s, c);
        }

        [GeometryShader(GeometryInputType.Point, 4)]
        void GS(VS_GS[] input, TriangleStream<PS_IN> spriteStream)
        {
            PS_IN v;
            VS_GS g = input[0];

            v.col = g.col;


            Matrix2x2 rot = GetRotation(input[0].rotation);
            Vector2 origin = g.origin.XY;
            Vector2 pos = g.pos.XY;
            Vector2 size = g.size.XY;
            Vector4 uv = g.uv;
            Vector2 p;


            // [unroll]
            for (int i = 0; i < 4; i++)
            {
                p = Mul(size * (spriteCorners[i] - origin), rot);
                p += pos;
                v.pos = new Vector4(p, 0, 1);
                v.pos = Mul(v.pos, wvp);
                v.uv.X = uv[uvTable[i].X];
                v.uv.Y = uv[uvTable[i].Y];
                v.uv.Z = g.arraySlice;
                spriteStream.Append(v);
            }
        }

        [GeometryShader(GeometryInputType.Point, 4)]
        void GS_Line(VS_GS[] input, TriangleStream<PS_IN> spriteStream)
        {
            PS_IN v;
            v.col = input[0].col;
            v.uv = new Vector3(0, 0, 0);
            Vector2 p1 = input[0].pos;
            Vector2 p2 = input[0].size;
            Vector2 dir = p2 - p1;
            Vector2 normal = Normalize(new Vector2(-dir.Y, dir.X));
            float thickness = input[0].rotation * 0.5f;

            // Vertex p1 vertex 0 (v0)
            v.pos = new Vector4(p1 - (thickness * normal), 0, 1);
            v.pos = Mul(v.pos, wvp);
            spriteStream.Append(v);

            // Vertex p1 vertex 1 (v1)
            v.pos = new Vector4(p1 + (thickness * normal), 0, 1);
            v.pos = Mul(v.pos, wvp);
            spriteStream.Append(v);

            // Vertex p2 vertex 0 (v2)
            v.col = input[0].uv;
            v.pos = new Vector4(p2 - (thickness * normal), 0, 1);
            v.pos = Mul(v.pos, wvp);
            spriteStream.Append(v);

            // Vertex p2 vertex 1 (v3)
            v.pos = new Vector4(p2 + (thickness * normal), 0, 1);
            v.pos = Mul(v.pos, wvp);
            spriteStream.Append(v);
        }

        [GeometryShader(GeometryInputType.Point, 66)]
        void GS_Circle(VS_GS[] input, TriangleStream<PS_IN> spriteStream)
        {
            PS_IN v;
            // center vertex
            v.col = input[0].col;
            v.uv = new Vector3(0, 0, 0);
            Vector4 center = new Vector4(input[0].pos, 0, 1);

            float segs = input[0].rotation;
            Vector2 radius = input[0].size;
            Vector2 startEnd = input[0].origin;
            float range = startEnd.Y - startEnd.X;
            float angleInc = degToRad360 / segs;
            float angle = startEnd.X;
            float remaining = range;

            float inc = 0;
            float doEdge = 0; // if 0, we place a center point for the strip to orient around.
            float vCount = (segs * 2) + 2;

            // [unroll]
            for (int i = 0; i < vCount; i++)
            {
                v.pos = center + (doEdge * new Vector4(Sin(angle) * radius.X, Cos(angle) * radius.Y, 0, 0));
                v.pos = Mul(v.pos, wvp);

                inc = Min(angleInc, remaining);
                angle += inc * doEdge;
                remaining -= inc * doEdge;
                doEdge = 1 - doEdge;
                spriteStream.Append(v);
            }
        }

        [GeometryShader(GeometryInputType.Point, 3)]
        void GS_Tri(VS_GS[] input, TriangleStream<PS_IN> spriteStream)
        {
            PS_IN v;
            v.col = input[0].col;
            v.uv = new Vector3(0, 0, 0);

            // p1
            v.pos = Mul(new Vector4(input[0].pos, 0, 1), wvp);
            spriteStream.Append(v);

            // p3
            v.pos = Mul(new Vector4(input[0].origin, 0, 1), wvp);
            spriteStream.Append(v);

            // p2
            v.pos = Mul(new Vector4(input[0].size, 0, 1), wvp);
            spriteStream.Append(v);
        }

        [Semantic(SemanticType.SV_Target)]
        Vector4 PS(PS_IN input)
        {
            Vector4 col = mapDiffuse.Sample(diffuseSampler, input.uv);
            return col * input.col;
        }

        [Semantic(SemanticType.SV_Target)]
        Vector4 PS_NoTexture(PS_IN input)
        {
            return input.col;
        }
    }
}