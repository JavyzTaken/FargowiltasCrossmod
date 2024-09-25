sampler screenTexture : register(s0);
sampler heatMetaballsTexture : register(s2);
sampler heatNoiseTexture : register(s3);

float globalTime;
float opacity;
float2 screenZoom;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float heatDistortionIntensity = tex2D(heatMetaballsTexture, (coords - 0.5) / screenZoom + 0.5);
    float heatDistortionAngle = tex2D(heatNoiseTexture, (coords - 0.5) / screenZoom * 0.5 + 0.5 + float2(0, globalTime * 0.03)).r * 16 + globalTime * 0.5;
    
    float2 heatDistortionDirection = float2(cos(heatDistortionAngle), sin(heatDistortionAngle));
    return tex2D(screenTexture, coords + heatDistortionDirection * heatDistortionIntensity * opacity * 0.0024);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
