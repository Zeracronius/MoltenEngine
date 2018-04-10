struct LIGHT
{
	float4x4 Transform;
	float3 Position;
	float RangeRcp;
	float3 Color;
	float Intensity;
	float3 Forward;
	float Tess; // Tessellation factor
	float Length;
	float HalfLength;
};

StructuredBuffer<LIGHT> LightData : register(t8);

//=========VERTEX SHADER=====================================================
float4 VS() : SV_Position
{
	return float4(0.0, 0.0, 0.0, 1.0);
}

//=========HULL SHADER ======================================================
struct HS_CONSTANT_DATA_OUTPUT
{
	float Edges[4] : SV_TessFactor;
	float Inside[2] : SV_InsideTessFactor;
};

HS_CONSTANT_DATA_OUTPUT LightConstantHS()
{
	HS_CONSTANT_DATA_OUTPUT Output;

	float tessFactor = 18.0; //TODO make this configurable as a light "quality" option
	Output.Edges[0] = Output.Edges[1] = Output.Edges[2] = Output.Edges[3] = tessFactor;
	Output.Inside[0] = Output.Inside[1] = tessFactor;

	return Output;
}

//=======PCF Shadows and PCSS================================================
// Poisson sampling
static const float2 poissonDisk[16] = {
	float2(-0.94201624, -0.39906216),
	float2(0.94558609, -0.76890725),
	float2(-0.094184101, -0.92938870),
	float2(0.34495938, 0.29387760),
	float2(-0.91588581, 0.45771432),
	float2(-0.81544232, -0.87912464),
	float2(-0.38277543, 0.27676845),
	float2(0.97484398, 0.75648379),
	float2(0.44323325, -0.97511554),
	float2(0.53742981, -0.47373420),
	float2(-0.26496911, -0.41893023),
	float2(0.79197514, 0.19090188),
	float2(-0.24188840, 0.99706507),
	float2(-0.81409955, 0.91437590),
	float2(0.19984126, 0.78641367),
	float2(0.14383161, -0.14100790)
};

Texture2D<float> mapShadow				: register(t6);
Texture2DArray<float> mapShadowCascade	: register(t6);
Texture2D<float> mapAOcclusion			: register(t7);
SamplerComparisonState PCFSampler		: register(s2);

cbuffer cbShadowMap : register(b3)
{
	float4x4 ToShadowmap		: packoffset(c0);
	float ShadowMapPixelSize : packoffset(c4);
	float LightSize : packoffset(c4.y);
}

// Shadow PCSS calculation helper function
float ShadowPCSS(SamplerState texSampler, float3 position)
{
	// Transform the world position to shadow projected space
	float4 posShadowMap = mul(float4(position, 1.0), ToShadowmap);

	// Transform the position to shadow clip space
	float3 UVD = posShadowMap.xyz / posShadowMap.w;

	// Convert to shadow map UV values
	UVD.xy = 0.5 * UVD.xy + 0.5;
	UVD.y = 1.0 - UVD.y;

	// Search for blockers
	float avgBlockerDepth = 0;
	float blockerCount = 0;

	[unroll]
	for (int i = -2; i <= 2; i += 2)
	{
		[unroll]
		for (int j = -2; j <= 2; j += 2)
		{
			float4 d4 = mapShadow.GatherRed(texSampler, UVD.xy, int2(i, j));
			float4 b4 = (UVD.z <= d4) ? 0.0 : 1.0;

			blockerCount += dot(b4, 1.0);
			avgBlockerDepth += dot(d4, b4);
		}
	}

	// Check if we can early out
	if (blockerCount <= 0.0)
	{
		return 1.0;
	}

	// Penumbra width calculation
	avgBlockerDepth /= blockerCount;
	float fRatio = ((UVD.z - avgBlockerDepth) * LightSize) / avgBlockerDepth;
	fRatio *= fRatio;

	// Apply the filter
	float att = 0;

	[unroll]
	for (i = 0; i < 16; i++)
	{
		float2 offset = fRatio * ShadowMapPixelSize.xx * poissonDisk[i];
		att += mapShadow.SampleCmpLevelZero(PCFSampler, UVD.xy + offset, UVD.z);
	}

	// Divide by 16 to normalize
	return att * 0.0625;
}

// Shadow PCF calculation helper function
float ShadowPCF(float3 position)
{
	// Transform the world position to shadow projected space
	float4 posShadowMap = mul(float4(position, 1.0), ToShadowmap);

	// Transform the position to shadow clip space
	float3 UVD = posShadowMap.xyz / posShadowMap.w;

	// Convert to shadow map UV values
	UVD.xy = 0.5 * UVD.xy + 0.5;
	UVD.y = 1.0 - UVD.y;

	// Compute the hardware PCF value
	return mapShadow.SampleCmpLevelZero(PCFSampler, UVD.xy, UVD.z);
}

const static float MAX_LUM = 32768.0;

//==============================PBR=======================================
float3 pbr_cook_torrance
	(
		in float3 normal, // normal
		in float3 toEye, // direction to eye/camera
		in float3 toLight, // direction to light
		in float3 cLightCol, // light color
		in float3 cDiffuse, // scene color
		in float3 cSpecular // specular color
		)
{
	// Sample the textures
	// NOTE: These are to be mapped to a PBR texture.
	float surfaceR = 0.1; // IoR
	float surfaceG = 0.2; // Roughness
	float surfaceB = 1; // Metallic

	float2  Roughness = float2(surfaceR, surfaceG);

	Roughness.r *= 3.0f;

	// Correct the input and compute aliases
	float3  ViewDir = normalize(toEye);
	float3  LightDir = normalize(toLight);
	float3  vHalf = normalize(LightDir + ViewDir);
	float  NormalDotHalf = dot(normal, vHalf);
	float  ViewDotHalf = dot(vHalf, ViewDir);
	float  NormalDotView = dot(normal, ViewDir);
	float  NormalDotLight = dot(normal, LightDir);

	// Compute the geometric term
	float  G1 = (2.0f * NormalDotHalf * NormalDotView) / ViewDotHalf;
	float  G2 = (2.0f * NormalDotHalf * NormalDotLight) / ViewDotHalf;
	float  G = min(1.0f, max(0.0f, min(G1, G2)));

	// Compute the fresnel term
	float  F = Roughness.g + (1.0f - Roughness.g) * pow(1.0f - NormalDotView, 5.0f);

	// Compute the roughness term
	float  R_2 = Roughness.r * Roughness.r;
	float  NDotH_2 = NormalDotHalf * NormalDotHalf;
	float  A = 1.0f / (4.0f * R_2 * NDotH_2 * NDotH_2);
	float  B = exp(-(1.0f - NDotH_2) / (R_2 * NDotH_2));
	float  R = A * B;

	// Compute the final term
	float3  S = cSpecular * ((G * F * R) / (NormalDotLight * NormalDotView));

	// Clamp specular into a reasonable range to prevent patchy bloom glitches.
	float3 finalS = saturate(S) * 4;
	finalS = min(finalS, S);

	float3  Final = cLightCol.rgb * max(0.0f, NormalDotLight) * (cDiffuse + finalS);

	return float4(Final, 1.0f);
}