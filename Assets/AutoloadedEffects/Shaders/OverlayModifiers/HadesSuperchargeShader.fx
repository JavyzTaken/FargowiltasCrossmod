sampler baseTexture : register(s0);
sampler electricNoiseTexture : register(s1);

float globalTime;
float electrifyInterpolant;
float4 electricityColor;

float Overlay(float a, float b)
{
    return lerp(a * b * 2, 1 - (1 - a) * (1 - b) * 2, a >= 0.5);
}
float4 Overlay(float4 a, float4 b)
{
    return float4(Overlay(a.r, b.r), Overlay(a.g, b.g), Overlay(a.b, b.b), max(a.a, b.a));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(baseTexture, coords);
    float4 color = Overlay(baseColor, electricityColor) * baseColor.a + pow(baseColor.r, 1.5) + pow(baseColor.b, 3) * 20;
    return lerp(baseColor, saturate(color), electrifyInterpolant);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}