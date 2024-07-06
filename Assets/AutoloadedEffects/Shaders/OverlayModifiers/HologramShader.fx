sampler baseTexture : register(s0);
sampler uvOffsetTexture : register(s1);

float globalTime;
float hologramInterpolant;
float hologramSinusoidalOffset;
float2 textureSize0;
float4 frameArea;

float OverlayBlend(float a, float b)
{
    if (a < 0.5)
        return a * b * 2;
    
    return 1 - (1 - a) * (1 - b) * 2;
}

float3 OverlayBlend(float3 a, float3 b)
{
    return float3(OverlayBlend(a.r, b.r), OverlayBlend(a.g, b.g), OverlayBlend(a.b, b.b));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Apply pixelation and hologram offset visuals.
    float2 pixelationFactor = 2 / textureSize0;
    coords = ceil(coords / pixelationFactor) * pixelationFactor;
    coords.x += sin((coords.y - 0.5) * 400 + hologramInterpolant * 20) * hologramSinusoidalOffset;
    coords = clamp(coords, frameArea.xy + pixelationFactor, frameArea.zw - pixelationFactor);
    
    // Blend towards cyan based on the hologram interpolant.
    float4 color = tex2D(baseTexture, coords);
    color.rgb = lerp(color.rgb, OverlayBlend(color.rgb, float3(0, 1.15, 1)), hologramInterpolant);
    
    // Perform basic edge detection.
    float left = tex2D(baseTexture, coords - float2(pixelationFactor.x, 0)).g;
    float right = tex2D(baseTexture, coords + float2(pixelationFactor.x, 0)).g;
    float up = tex2D(baseTexture, coords - float2(0, pixelationFactor.y)).g;
    float down = tex2D(baseTexture, coords + float2(0, pixelationFactor.y)).g;
    float dx = (right - left) * 0.5;
    float dy = (down - up) * 0.5;
    float edge = sqrt(dx * dx + dy * dy);
    
    // Combine everything together.
    float edgeBrightness = 1 + smoothstep(0.25, 0.36, edge) * hologramInterpolant * 3;    
    return color * sampleColor * edgeBrightness;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}