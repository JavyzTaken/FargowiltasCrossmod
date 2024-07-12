sampler baseTexture : register(s0);
sampler noiseTextureA : register(s1);
sampler noiseTextureB : register(s2);

float globalTime;
float lifetimeRatio;
float spread;
float2 textureSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = 2 / textureSize;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float2 offsetFromBottom = coords - float2(0.5, 1);
    float2 directionFromBottom = normalize(offsetFromBottom);
    
    float opacity = smoothstep(1 - spread, 1, abs(directionFromBottom.y)) * pow(smoothstep(0.75, 0, length(offsetFromBottom)), 1.5);
    float4 color = tex2D(baseTexture, coords) * opacity;
    
    return color * sampleColor;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}