sampler baseTexture : register(s0);

float globalTime;
float scrollSpeed;
float gradientCount;
float3 gradient[8];

float3 PaletteLerp(float interpolant)
{
    int startIndex = clamp(interpolant * gradientCount, 0, gradientCount - 1);
    int endIndex = clamp(startIndex + 1, 0, gradientCount - 1);
    return lerp(gradient[startIndex], gradient[endIndex], frac(interpolant * gradientCount));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 baseData = tex2D(baseTexture, coords);
    float hueInterpolant = frac(globalTime * scrollSpeed + baseData.r);
    float3 color = PaletteLerp(hueInterpolant) * baseData.g;

    return float4(color, 1) * baseData.a * sampleColor * 1.2;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}