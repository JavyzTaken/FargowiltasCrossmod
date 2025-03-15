sampler baseTexture : register(s0);

float globalTime;
float blurriness;
float fadeToBackground;
float posterizationLevel;
float2 pixelationFactor;
float4 backgroundColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float4 color = 0;
    for (int i = -5; i < 5; i++)
        color += tex2D(baseTexture, coords + float2(i, 0) * blurriness * 0.006) * 0.1;
    
    color = float4(round(color.rgb * posterizationLevel) / posterizationLevel, 1) * color.a;
    
    return lerp(float4(color.rgb, 1), backgroundColor, fadeToBackground) * sampleColor * color.a;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
