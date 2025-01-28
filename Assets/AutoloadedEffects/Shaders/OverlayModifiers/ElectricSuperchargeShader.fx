sampler baseTexture : register(s0);
sampler electricNoiseTexture : register(s1);

float globalTime;
float glowInterpolant;
float2 textureSize;
float4 electricColor;
float4 frame;

float OverlayBlend(float a, float b)
{
    return lerp(a * b * 2, 1 - (1 - a) * (a - b) * 2, a >= 0.5);
}

float3 OverlayBlend(float3 a, float3 b)
{
    return float3(OverlayBlend(a.r, b.r), OverlayBlend(a.g, b.g), OverlayBlend(a.b, b.b));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate electric noise.
    float2 framedCoords = (coords * textureSize - frame.xy) / frame.zw;
    float2 polar = float2(distance(framedCoords, 0.5), atan2(framedCoords.y - 0.5, framedCoords.x - 0.5) / 6.283 + 0.5);
    float electricNoise = tex2D(electricNoiseTexture, polar * 2 + float2(globalTime * -0.5, globalTime)) + tex2D(electricNoiseTexture, framedCoords * 0.5 + float2(0, globalTime * 2.56)) * 1.3;
    
    // Apply positional offsets based on the electric noise, distorting the results slightly.
    float angle = electricNoise * 11;
    float2 warpDirection = float2(cos(angle), sin(angle));
    coords += warpDirection * glowInterpolant / textureSize * 2;
    
    // Apply strong glows over everything.
    float2 offsetFromCenter = 0.5 - framedCoords;
    float distanceFromCenter = sqrt(pow(offsetFromCenter.x, 2) + pow(offsetFromCenter.y, 2) * 1.5) + electricNoise * 0.1;
    float distanceFromEdge = distance(distanceFromCenter, 0.4);    
    float4 baseColor = tex2D(baseTexture, coords) * sampleColor;
    float4 glowIntensity = 1 + electricColor * (pow(baseColor.a / distanceFromEdge * 0.285, 1.2) + (1 - distanceFromCenter) * 1.1);
    float4 glowColor = saturate(baseColor * glowIntensity);
    
    return lerp(baseColor, glowColor, glowInterpolant);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}