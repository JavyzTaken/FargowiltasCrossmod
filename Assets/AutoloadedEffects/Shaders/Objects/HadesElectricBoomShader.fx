sampler baseTexture : register(s0);
sampler noiseTextureA : register(s1);
sampler noiseTextureB : register(s2);

float globalTime;
float lifetimeRatio;
float2 textureSize0;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = textureSize0 * 0.35;
    coords = floor(coords * pixelationFactor) / pixelationFactor;
    
    // Calculate radius slice values for the explosion.
    float endingRadius = sqrt(lifetimeRatio) * 0.5;
    float distanceFromCenter = distance(coords, 0.5);
    float radiusInterpolant = smoothstep(-0.1, min(endingRadius, 0.51), distanceFromCenter);
    
    // Calculate polar coordinates in advance. It will be used for noise calculations later.
    float angleFromCenter = atan2(coords.y - 0.5, coords.x - 0.5);
    float2 polar = float2(angleFromCenter / 3.141 + 0.5, distanceFromCenter);
    
    // Calculate glow values.
    bool betweenExplosionRadius = distanceFromCenter <= endingRadius;
    float generalNoise = sin(polar.y * 6.34589 - globalTime * 1.4 + angleFromCenter * 20) * sin(polar.y * -11.07211 + globalTime * 0.4815 + angleFromCenter * -5) * 0.5 + 0.5;
    float glowNoise1 = tex2D(noiseTextureA, polar * float2(0.5, 2) + float2(globalTime * 0.3, 0.05 - globalTime * 0.25) - generalNoise * 0.04);
    float glowNoise2 = tex2D(noiseTextureB, polar * float2(0.5, 7) + float2(-globalTime * 0.3, 0.29 - globalTime) + generalNoise * 0.025);
    float glowNoise = glowNoise1 - glowNoise2;
    float edgeAntialiasingOpacity = smoothstep(1, 0.995 - generalNoise * lifetimeRatio * 0.07, radiusInterpolant);
    
    // Calculate the final color.
    float4 color = pow(float4(sampleColor.rgb, 1) / (radiusInterpolant + 0.15), 1.25) + smoothstep(0.6, 0.99, radiusInterpolant) + glowNoise - float4(1, 1, 1, 0) * (1 - radiusInterpolant);
    color += smoothstep(0.45, 0.5, distanceFromCenter) * 1.2;

    return color * edgeAntialiasingOpacity * sampleColor.a;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}