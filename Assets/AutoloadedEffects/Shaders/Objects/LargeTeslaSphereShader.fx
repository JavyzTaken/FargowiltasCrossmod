sampler baseTexture : register(s0);
sampler edgeShapeNoiseTexture : register(s1);
sampler innerElectricityNoiseTexture : register(s2);

float globalTime;
float sphereSpinScrollOffset;
float posterizationPrecision;
float ridgeNoiseInterpolationStart;
float ridgeNoiseInterpolationEnd;
float2 textureSize0;

float2 GetFakeSphereCoords(float2 coords)
{
    float2 coordsNormalizedToCenter = (coords - 0.5) * 2;
    float distanceFromCenterSqr = dot(coordsNormalizedToCenter, coordsNormalizedToCenter) * 2;
    
    // Calculate coordinates relative to the sphere.
    // This pinch factor effectively ensures that the UVs are relative to a circle, rather than a rectangle.
    // This helps SIGNIFICANTLY for making the texturing look realistic, as it will appear to be traveling on a
    // sphere rather than on a sheet that happens to overlay a circle.
    float spherePinchFactor = (1 - sqrt(abs(1 - distanceFromCenterSqr))) / distanceFromCenterSqr + 0.001;
    
    // Exaggerate the pinch slightly.
    spherePinchFactor = pow(spherePinchFactor, 1.5);
    
    float2 sphereCoords = frac((coords - 0.5) * spherePinchFactor + 0.5);
    return sphereCoords;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the results.
    float2 pixelationFactor = textureSize0 * 0.7;
    coords = floor(coords * pixelationFactor) / pixelationFactor;
     
    // Calculate the base color based on distance from the center.
    float distanceFromCenter = distance(coords, 0.5) + tex2D(edgeShapeNoiseTexture, coords * 1.5 + float2(0, globalTime)) * 0.02;
    float brightness = smoothstep(0.5, 0.1, distanceFromCenter) / distanceFromCenter * 0.4;
    float4 color = saturate(sampleColor * brightness);
    
    float2 sphereCoords = frac(GetFakeSphereCoords(coords) + float2(-sphereSpinScrollOffset, 0));
    
    float innerNoise = 0;
    float innerNoiseWarpAngle = tex2D(innerElectricityNoiseTexture, coords + float2(globalTime * 0.3, 0)).r * 16;
    float2 innerNoiseWarpOffset = float2(cos(innerNoiseWarpAngle), sin(innerNoiseWarpAngle)) * 2 / textureSize0;
    for (int i = 0; i < 4; i++)
    {
        float2 noiseInfluence = float2(innerNoise * 0.3, 0);
        innerNoise = tex2D(innerElectricityNoiseTexture, sphereCoords * pow(2, i) * 0.5 + noiseInfluence + innerNoiseWarpOffset).r;
    }
    
    // Add detail to the core of the texture.
    float ridgeNoise = smoothstep(ridgeNoiseInterpolationStart, ridgeNoiseInterpolationEnd, 1 - abs(innerNoise));
    color -= ridgeNoise * pow(color.a, 4.16) * float4(1, -0.15, 0, 0) * 0.09;
    
    // Apply color posterization.
    color = float4(floor(color.rgb * posterizationPrecision) / posterizationPrecision, 1) * color.a;
    
    // Brighten edges so that sparks draw at full brightness.
    color.a = lerp(color.a, 0, smoothstep(0.15, 0.4, distanceFromCenter));
    
    return color;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}