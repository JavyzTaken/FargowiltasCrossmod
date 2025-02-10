sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float pixelationLevel;
float blurInterpolant;
float globalTime;
float glowInterpolant;
float turbulence;
float2 textureSize;
float3 glowColor;
float3 translucentAccent;
float4 frame;

float gaussianFactors[6] = { 0.289, 0.252, 0.166, 0.083, 0.03, 0.0091 };

float OverlayBlendIndividual(float top, float bottom)
{
    return lerp(top * bottom * 2, 1 - (1 - top) * (1 - bottom) * 2, top >= 0.5);
}

float3 OverlayBlend(float3 top, float3 bottom)
{
    return float3(OverlayBlendIndividual(top.r, bottom.r), OverlayBlendIndividual(top.g, bottom.g), OverlayBlendIndividual(top.b, bottom.b));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * textureSize - frame.xy) / frame.zw;
    framedCoords = round(framedCoords * pixelationLevel) / pixelationLevel;
    
    float2 mouthOrigin = float2(0.86, 0.6);
    float2 polar = float2(atan2(framedCoords.y - mouthOrigin.y, framedCoords.x - mouthOrigin.x), distance(framedCoords, mouthOrigin));
    polar.x += polar.y * 3;

    float noise = tex2D(noiseTexture, polar * float2(1, 2.9) + globalTime * float2(-0.3, 0) + sampleColor.a * 2) - 0.5;
    noise = tex2D(noiseTexture, polar * float2(1, 3) + globalTime * float2(0.6, 0) + noise * turbulence);
    
    float4 baseTextureData = 0;
    for (int i = -5; i < 5; i++)
        baseTextureData += tex2D(baseTexture, coords + float2(i, 0) * blurInterpolant * 0.017) * gaussianFactors[abs(i)] * 0.5;
    
    float3 baseColor = baseTextureData.rgb * sampleColor.rgb;
    float3 color = OverlayBlend(baseColor, glowColor + noise * 0.9) + translucentAccent * smoothstep(1.05, 0.35, sampleColor.a) * 0.3;
    
    float opacity = sampleColor.a * baseTextureData.a;
    return lerp(baseTextureData * sampleColor, float4(color, 1) * opacity, glowInterpolant);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
