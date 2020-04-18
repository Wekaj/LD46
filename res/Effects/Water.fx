#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
SamplerState SpriteTextureSampler
{
	Texture = <SpriteTexture>;
};

Texture2D WaterMask;
SamplerState WaterMaskSampler
{
	Texture = <WaterMask>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 mask = WaterMask.Sample(WaterMaskSampler, input.TextureCoordinates);

	float2 offset = float2(mask.a * 0.1, mask.a * 0.1);

	float4 color = SpriteTexture.Sample(SpriteTextureSampler, input.TextureCoordinates + offset) * input.Color;

	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};