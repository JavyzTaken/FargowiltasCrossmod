sampler screenTexture : register(s0);
sampler exoTwinsTargetTexture : register(s1);
sampler dustTargetTexture : register(s2);

float globalTime;
float impactFrameInterpolant;
float4 darkFrameColor;
float4 lightFrameColor;
float4x4 contrastMatrix;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(screenTexture, coords);
    float4 highContrastColor = mul(baseColor, contrastMatrix);
    
    float luminosity = dot(highContrastColor.rgb, float3(0.3, 0.6, 0.1));
    float4 impactFrameColor = lerp(lightFrameColor, darkFrameColor, any(tex2D(exoTwinsTargetTexture, coords) + tex2D(dustTargetTexture, coords)));
    return lerp(baseColor, impactFrameColor, impactFrameInterpolant);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
