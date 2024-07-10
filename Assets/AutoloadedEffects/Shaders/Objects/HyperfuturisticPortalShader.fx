sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);
sampler distanceTexture : register(s2);

bool useTextureForDistanceField;
float globalTime;
float scale;
float biasToMainSwirlColorPower;
float2 textureSize0;
float3 mainSwirlColor;
float3 secondarySwirlColor;

float3 ColorBurn(float3 a, float3 b)
{
    return 1 - (1 - b) / a;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = 2 / textureSize0;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    // Calculate polar coordinates.
    float distanceFromCenter = distance(coords, 0.5);
    float angleFromCenter = atan2(coords.y - 0.5, coords.x - 0.5);
    float2 polar = float2(distanceFromCenter, angleFromCenter / 3.141 + 0.5);
    
    // Calculate two distance values that are interpolated between when calculating the edge shape of the portal.
    float noisyDistance = (tex2D(noiseTexture, polar * 2 + float2(0.9, 1.3) * globalTime) * 0.06 + 0.36);
    float fadeOutDistance = pow(cos(angleFromCenter * 2 + globalTime * -20), 2) * pow(cos(angleFromCenter + globalTime * 8), 2) * 0.4;
    fadeOutDistance = lerp(fadeOutDistance, tex2D(distanceTexture, coords), useTextureForDistanceField);
    
    // Interpolate between the distance values to calculate the distance to the edge of the portal.
    float distanceToEdge = lerp(fadeOutDistance, noisyDistance, pow(scale, 3)) * scale;
    
    // Use the above calculations to determine a swirl color within the rift.
    float innerColorInterpolant = smoothstep(distanceToEdge, distanceToEdge * 0.7, distanceFromCenter);
    float swirlNoise = pow(tex2D(noiseTexture, polar * 4 + float2(0, distanceFromCenter * -12 - globalTime * -7)), biasToMainSwirlColorPower);
    float3 swirlColor = lerp(mainSwirlColor, secondarySwirlColor, swirlNoise) * pow(smoothstep(distanceToEdge, 0, distanceFromCenter), 2) * 2;
    float4 color = float4(swirlColor, 1) * innerColorInterpolant;
    
    return color * sampleColor;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}