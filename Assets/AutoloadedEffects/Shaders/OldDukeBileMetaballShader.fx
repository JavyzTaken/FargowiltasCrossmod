sampler metaballContents : register(s0);
sampler overlayTexture : register(s1);
sampler dissolveNoiseTexture : register(s2);
sampler bloodTexture : register(s3);

float globalTime;
float gradientCount;
float dissolvePersistence;
float2 screenSize;
float2 layerSize;
float2 layerOffset;
float2 singleFrameScreenOffset;
float2 maxDistortionOffset;
float3 gradient[5];

float3 PaletteLerp(float interpolant)
{
    int startIndex = clamp(interpolant * gradientCount, 0, gradientCount - 1);
    int endIndex = clamp(startIndex + 1, 0, gradientCount - 1);
    return lerp(gradient[startIndex], gradient[endIndex], frac(interpolant * gradientCount));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the UV position of the current pixel relative to the world.
    float2 worldUV = (coords + layerOffset + singleFrameScreenOffset) * screenSize / layerSize;
    
    // Calculate a distortion offset value based on two noise values on the X and Y axis.
    float2 uvOffsetNoiseCoords = worldUV * float2(1.6, 0.1);
    float2 uvOffsetDirection = (float2(tex2D(overlayTexture, uvOffsetNoiseCoords).r, tex2D(overlayTexture, uvOffsetNoiseCoords + 0.33).r) - 0.5);
    float2 uvOffset = uvOffsetDirection * maxDistortionOffset / screenSize;
    
    // Sample color data at the distorted position.
    // The Blue channel encodes the lifetime ratio of the blood particle at the given pixel.
    float4 colorData = tex2D(metaballContents, coords + uvOffset);    
    float lifetimeRatio = colorData.b;
    
    // Use the lifetime ratio, along with some noise, to calculate how much this pixel should be dissolved.
    float distanceFromCenter = colorData.r;
    float dissolveThreshold = 0.8 - lifetimeRatio * dissolvePersistence - (0.7 - distanceFromCenter);
    float dissolveNoise = tex2D(dissolveNoiseTexture, worldUV * 7.5);
    float dissolveOpacity = smoothstep(dissolveThreshold, dissolveThreshold + 0.15, dissolveNoise);
    
    // Use the lifetime ratio, along with a little bit of noise again, to calculate the hue of this pixel.
    // This is used to make blood become darker and drier as it ages.
    float2 bloodTextureOffset = (tex2D(overlayTexture, uvOffsetNoiseCoords + globalTime * -0.1) - 0.5) * 0.03;
    bloodTextureOffset += globalTime * float2(0.012, -0.003);
    
    float darkening = colorData.g - tex2D(bloodTexture, worldUV * 1.5 + lifetimeRatio * 0.05) * 0.1;
    float hue = smoothstep(0.75, 0, lifetimeRatio) + darkening * 0.4 + (0.7 - distanceFromCenter) * 0.04;
    
    // Combine color and opacity.
    float opacity = dissolveOpacity * colorData.a;
    return float4(PaletteLerp(hue), 1) * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}