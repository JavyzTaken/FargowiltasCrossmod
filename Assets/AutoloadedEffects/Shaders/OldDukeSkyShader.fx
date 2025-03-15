sampler baseTexture : register(s0);
sampler lightningCrackTexture : register(s1);
sampler distanceDistortionTexture : register(s2);

bool lightningEnabled;
float lightningFlashLifetimeRatios[4];
float2 lightningFlashPositions[4];
float lightningFlashIntensities[4];
float3 lightningColor;
float2 pixelationFactor;

float3 CalculateLightningColor(float4 baseColor, float2 coords)
{
    float flashIntensity = 0;
    float overallFlashIntensity = 0;
    
    for (int i = 0; i < 4; i++)
    {
        float lifetimeRatio = lightningFlashLifetimeRatios[i];
        float distanceDistortion = tex2D(distanceDistortionTexture, coords * 3 + float2(cos(lifetimeRatio * 10 + coords.y * 24) * 0.03, lifetimeRatio * -0.2)) - 0.5;
        
        // Calculate the offset from the flash, biasing the X coordinates based on time, to make it look like the effect is reaching downward over time.
        float squish = smoothstep(0, 0.9, lifetimeRatio);
        float2 offsetFromFlash = lightningFlashPositions[i] - coords;
        offsetFromFlash.x *= lerp(1, 6, squish);
        
        // Calculate the distance from the flash center, with the aforementioned squished offset and a distance distortion that ensures the effect doesn't look perfectly circular.
        float localDistanceFromFlash = length(offsetFromFlash) - distanceDistortion * lifetimeRatio * 0.4;
        
        // Calculate the intensity of the flash, sharply rising and then slowly dissipating.
        float localFlashIntensity = smoothstep(0, 0.1, lifetimeRatio) * smoothstep(1, 0.1, lifetimeRatio);
        
        // Calculate the brightness of lightning based on crack noise.
        float2 lightningCoords = coords * float2(4.5, 2) + lightningFlashPositions[i] * 1000 + distanceDistortion * 0.1;
        float lightningNoise = smoothstep(-0.1, 1 - lifetimeRatio * 0.5, tex2D(lightningCrackTexture, lightningCoords)) + 0.01;
        float lightningIntensity = 0.7 / lightningNoise * (1 - lifetimeRatio);
        
        // Make the intensity of lightning reach a hard cutoff based on distance from the flash source.
        lightningIntensity *= smoothstep(2, 0, localDistanceFromFlash / lightningFlashIntensities[i]);
        
        // Increment brightness.
        flashIntensity += localFlashIntensity / max(0.0001, pow(localDistanceFromFlash, 1.8)) * lightningIntensity;
        
        overallFlashIntensity += localFlashIntensity;
    }
    
    // Apply the flash intensity based on how bright the original pixel was.
    // Darker pixels are affected less to ensure that depth is preserved.
    float originalBrightness = dot(baseColor.rgb, float3(0.3, 0.6, 0.1));
    float red = lerp(0.7, 0.1, tex2D(baseTexture, coords * 1.1));
    float interpolateFactor = smoothstep(0.05, 0.65, originalBrightness) * 0.25;
    float3 result = lerp(baseColor.rgb, lightningColor, max(flashIntensity, 0) * interpolateFactor) + overallFlashIntensity * baseColor.a * 0.75;
    
    return lerp(baseColor.rgb, result, lightningEnabled);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float4 color = tex2D(baseTexture, coords);
    return float4(CalculateLightningColor(color, coords), 1) * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
