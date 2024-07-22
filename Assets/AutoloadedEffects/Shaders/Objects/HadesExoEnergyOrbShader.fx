sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float globalTime;
float pulseIntensity;
float glowIntensity;
float glowPower;
float2 textureSize0;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = textureSize0 * 0.5;
    coords = floor(coords * pixelationFactor) / pixelationFactor;
    
    float distanceFromCenter = distance(coords, 0.5);
    float2 polar = float2(distanceFromCenter, atan2(coords.y - 0.5, coords.x - 0.5) / 3.141 + 0.5);
    
    // Calculate the pulse and glow values.
    float pulse = cos(globalTime * 45 + polar.x * 12) * 0.5 + 0.5;    
    float distanceNoise = tex2D(noiseTexture, polar * float2(0.7, 1) - globalTime);
    float distortedDistance = distance(coords, 0.5) + distanceNoise * 0.03 + pulse * pulseIntensity;
    float glow = glowIntensity / pow(distortedDistance, glowPower);
    
    // Calculate the final color.
    float4 color = saturate(sampleColor * (glow + 1)) * smoothstep(0.5, 0.4, distortedDistance);
    color = saturate(color);
    
    // Make the edges a bit more natural in their blue tones.
    color.rg -= smoothstep(0.3, 0.4, distanceFromCenter) * 0.3;
    color.g += smoothstep(0.39, 0.47, distanceFromCenter) * color.a * 0.4;
    
    // Apply posterization.
    color = floor(color * 10) / 10;
    
    return color;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}