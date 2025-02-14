sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float globalTime;
float lifetimeRatio;
float2 textureSize0;

// I HATE YOU you SILLY CompilationFailed errors WHY do you EXIST???
float RealSmoothstep(float x)
{
    return x * x * 3 - x * x * x * 2;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = textureSize0 * 0.7;
    coords = floor(coords * pixelationFactor) / pixelationFactor;
    
    // Calculate radius slice values for the explosion.
    float endingRadius = sqrt(lifetimeRatio) * 0.5;
    float startingRadius = endingRadius - lerp(0.32, 0.29, lifetimeRatio);
    float distanceFromCenter = distance(coords, 0.5);
    float radiusInterpolant = smoothstep(startingRadius, min(endingRadius, 0.51), distanceFromCenter);
    
    // Calculate polar coordinates in advance. It will be used for noise calculations later.
    float2 polar = float2(atan2(coords.y - 0.5, coords.x - 0.5) / 3.141 + 0.5, distanceFromCenter);
    
    // Determine whether the pixel needs to be erased, based on noise and pixel distance from the center.
    bool betweenExplosionRadius = distanceFromCenter >= startingRadius && distanceFromCenter <= endingRadius;
    float erasureNoise = tex2D(noiseTexture, polar * float2(5, 1));
    float noiseErasureThreshold = 1 - (1 - radiusInterpolant);
    float noiseErasureInterpolant = smoothstep(0.85, 1, erasureNoise / noiseErasureThreshold);
    bool eraseFromWithin = distanceFromCenter < lerp(0.85, 1.2, erasureNoise) * pow(lifetimeRatio, 1.9) * 0.54;
    bool erasePixel = distanceFromCenter >= lerp(0.49, 0.5, 1 - erasureNoise) || !betweenExplosionRadius || eraseFromWithin;
    
    // Calculate glow values.
    float glowNoise1 = tex2D(noiseTexture, polar * float2(1, 3) + float2(globalTime, 0.05));
    float glowNoise2 = tex2D(noiseTexture, polar * float2(2, 1) + float2(-globalTime, 0.29));
    float glowNoise = glowNoise1 - glowNoise2;
    float edgeAntialiasingOpacity = smoothstep(1, 0.995, radiusInterpolant);
    
    // Calculate the final color, incorporating above erasure conditions.
    float4 color = pow(sampleColor / (radiusInterpolant + 0.2), 1.25) + smoothstep(0.7, 0.99, radiusInterpolant) + glowNoise - float4(1, 1, 1, 0) * (1 - radiusInterpolant);
    color += smoothstep(0.45, 0.5, distanceFromCenter) * 2;
    
    return color * (1 - erasePixel) * (1 - noiseErasureInterpolant) * edgeAntialiasingOpacity;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}