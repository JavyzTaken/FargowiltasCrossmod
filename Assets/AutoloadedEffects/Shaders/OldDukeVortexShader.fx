sampler noiseTextureA : register(s1);
sampler noiseTextureB : register(s2);

float globalTime;
float pixelationLevel;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Apply pixelation.
    coords = round(coords * pixelationLevel) / pixelationLevel;
    
    // Calculate polar coordinates.
    float2 offsetFromCenter = coords - 0.5;
    float2 polar = float2(atan2(offsetFromCenter.y, offsetFromCenter.x) / 6.283 + 0.5, length(offsetFromCenter));
    
    // Create a spiral by making the angular component of the polar coordinates increase with distance.
    polar.x += polar.y * 2.5;
    
    // Calculate an opacity value that cuts out the vortex's edge, leaving a generally circular shape.
    float edgeOffset = tex2D(noiseTextureA, polar + globalTime * float2(-0.7, 0.5)) * 0.15;
    float edgeFade = smoothstep(0.5, 0.3, polar.y + edgeOffset);
    
    // Calculate two spinning noise values.
    float leftSpin = tex2D(noiseTextureA, polar + globalTime * float2(0, 0.7));
    float rightSpin = tex2D(noiseTextureB, polar * float2(1, 2) + globalTime * float2(0, 0.5));
    float spin = (leftSpin + rightSpin) * 0.5;
    
    // Combine the noise value with the sample color and posterize.
    float4 color = saturate(float4(spin, spin, spin, 1) / (1.1 - float4(sampleColor.rgb, 1)));
    color = round(color * 8) / 8;
    
    // Combine everything together, adding a bright white core to the vortex to define it better.
    float innerCore = smoothstep(0.24, 0.07, polar.y) * sampleColor.a;    
    return color * edgeFade * smoothstep(0.01, 0.2, spin) * sampleColor.a + innerCore;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
