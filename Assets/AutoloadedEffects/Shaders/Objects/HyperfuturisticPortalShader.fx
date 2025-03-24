sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);
sampler distanceTexture : register(s2);

bool useTextureForDistanceField;
float globalTime;
float scale;
float biasToMainSwirlColorPower;
float gradientCount;
float2 textureSize0;
float3 gradient[8];

float3 PaletteLerp(float interpolant)
{
    int startIndex = clamp(interpolant * gradientCount, 0, gradientCount - 1);
    int endIndex = clamp(startIndex + 1, 0, gradientCount - 1);
    return lerp(gradient[startIndex], gradient[endIndex], frac(interpolant * gradientCount));
}

float EdgeDistance(float2 coords, float angleFromCenter)
{
    float n = 6;
    float modAngle = angleFromCenter % (6.283 / n);    
    float polygonEquation = 1 / cos(modAngle - 3.141 / n);
    return polygonEquation * 0.4;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = 2 / textureSize0;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    // Calculate polar coordinates.
    float distanceFromCenter = distance(coords, 0.5);
    float angleFromCenter = atan2(coords.y - 0.5, coords.x - 0.5) + 6.283;
    float2 polar = float2(distanceFromCenter, angleFromCenter / 3.141 + 0.5);
    
    // Calculate two distance values that are interpolated between when calculating the edge shape of the portal.
    float noisyDistance = (tex2D(noiseTexture, polar * 2 + float2(2.9, 0) * globalTime) * 0.06 + 0.36);
    float fadeOutDistance = EdgeDistance(coords, angleFromCenter);
    fadeOutDistance = lerp(fadeOutDistance, tex2D(distanceTexture, coords), useTextureForDistanceField);
    
    // Interpolate between the distance values to calculate the distance to the edge of the portal.
    float distanceToEdge = lerp(fadeOutDistance, noisyDistance, pow(scale, 4)) * scale;
    
    // Use the above calculations to determine a swirl color within the rift.
    float innerColorInterpolant = smoothstep(distanceToEdge, distanceToEdge * 0.7, distanceFromCenter);
    float swirlNoise = pow(tex2D(noiseTexture, polar * float2(3, 0) + float2(0, globalTime * 2)), biasToMainSwirlColorPower);
    float3 swirlColor = PaletteLerp(swirlNoise) * pow(smoothstep(distanceToEdge * 1.2, 0, distanceFromCenter), 2) * 2;
    float4 color = float4(swirlColor, 1) * innerColorInterpolant + smoothstep(0.2, 0.08, distanceFromCenter / scale) * 0.3;
    
    return saturate(color) * sampleColor;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}