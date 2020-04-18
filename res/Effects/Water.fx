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

Texture2D FlowMap;
SamplerState FlowMapSampler
{
	Texture = <FlowMap>;
};

float Time;

float4 WaterColor;

float2 Camera;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 flowPos = input.TextureCoordinates + Camera;

	float mask = ceil(WaterMask.Sample(WaterMaskSampler, input.TextureCoordinates).a);
	float2 flow = FlowMap.Sample(FlowMapSampler, flowPos * 2.0 + float2(0.2, 0.2) * Time).xy + float2(-0.5, -0.5); 

	float2 offset = flow * mask / 40.0;
	
	float4 original = SpriteTexture.Sample(SpriteTextureSampler, input.TextureCoordinates + offset) * input.Color;

	return original * (1.0 - mask * 0.75) + WaterColor * mask * 0.75;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};