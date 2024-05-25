sampler baseTexture : register(s0);

bool invertedGravity;
float globalTime;
float cloudDensity;
float horizontalOffset;
float cloudExposure;
float pixelationFactor;
float2 parallax;
float2 screenSize;
float2 worldPosition;
float3 sunPosition;
float lightningIntensities[10];
float2 lightningPositions[10];

float2 GetRayBoxIntersectionOffsets(float3 rayOrigin, float3 rayDirection, float3 boxMin, float3 boxMax)
{
    // Add a tiny nudge to the ray direction, since the compiler gets upset about the potential for division by zero otherwise.
    rayDirection += 1e-8;
    
    float3 tMin = (boxMin - rayOrigin) / rayDirection;
    float3 tMax = (boxMax - rayOrigin) / rayDirection;
    
    float3 t1 = min(tMin, tMax);
    float3 t2 = max(tMin, tMax);
    
    float tNear = max(max(t1.x, t1.y), t1.z);
    float tFar = min(min(t2.x, t2.y), t2.z);
    
    return float2(tNear, tFar);
}

// Density corresponds to how compact one can expect the cloud to be at a given point.
float CalculateCloudDensityAtPoint(float3 p)
{
    // Apply pixelation.
    p = round(p / pixelationFactor) * pixelationFactor;
    
    // Store the XY world position of this point for calculations later on.
    float2 localWorldPosition = p.xy + worldPosition;
    
    // Apply parallax.
    p.xy += worldPosition * parallax;
    
    // Convert input position from screen space coordinates to UV coordinates.
    p /= float3(screenSize.xy, 1);
    
    p.xy *= 1 - p.z;
    
    // Move horizontally based on wind.
    p.x += horizontalOffset;
    
    // Squish the noise, so that it looks more like a condensed cloud.
    p.xy *= float2(0.5, 1);
    
    // Calculate a UV offset for the given point.
    // This will be used to accent the clouds and make it look less like it's just a scrolling noise texture.
    float2 uvOffset = tex2D(baseTexture, p.xy * 0.9 + globalTime * 0.07) * 0.025;
    
    // Acquire density data from the three color channels of the noise. This uses the UV offset from above.
    float3 densityData = tex2D(baseTexture, p.xy * 0.2 + uvOffset);
    
    // Sample two density values from the color channels. These will be interpolated between based on the Z position, to create the illusion of
    // 3D noise without having to dedicate a ton of memory to a cube texture of it.
    float densityA = densityData.r;
    float densityB = densityData.b;
    float density = lerp(densityA, densityB, sin(p.z * 6.283) * 0.5 + 0.5);
    
    // Add secondary cloud density values in accordance with the general cloud density shader parameter.
    density += densityData.g * cloudDensity - p.y * (0.1 - cloudDensity) * 0.5 - uvOffset * 5;
    
    // Combine things together.
    return density * cloudDensity * lerp(0.15, 0.9, cloudDensity);
}

// Optical depth in this context basically is a measure of how much air is present along a given ray.
float CalculateOpticalDepth(float3 rayOrigin, float3 rayDirection, float rayLength, float numOpticalDepthPoints)
{
    float3 densitySamplePoint = rayOrigin;
    float stepSize = rayLength / (numOpticalDepthPoints - 1);
    float opticalDepth = 0;

    for (int i = 0; i < numOpticalDepthPoints; i++)
    {
        float localDensity = CalculateCloudDensityAtPoint(densitySamplePoint);
        opticalDepth += localDensity * stepSize;
        densitySamplePoint += rayDirection * stepSize;
    }
    return opticalDepth;
}

float4 CalculateScatteredLight(float3 rayOrigin, float3 rayDirection)
{
    float3 boxMin = float3(-999999, -999999, 0);
    float3 boxMax = float3(999999, 999999, 2);
    
    float inScatterPoints = 4;
    float sunIntersectionPoints = 4;
    float2 intersectionDistances = GetRayBoxIntersectionOffsets(rayOrigin, rayDirection, boxMin, boxMax);
    
    // Calculate how far the cloud intersection must travel.
    // If no intersection happened, simply return 0.
    float cloudIntersectionLength = intersectionDistances.y - intersectionDistances.x;
    if (cloudIntersectionLength <= 0)
        return 0;
    
    // Calculate how much each step along the in-scatter ray must travel.
    float inScatterStep = cloudIntersectionLength / (inScatterPoints - 1);
    
    // Initialize the light accumulation value at 0.
    float4 light = 0;
    
    // Start the in-scatter sample position at the edge of the box.
    // This process attempts to discretely model the integral used along the ray in real-world atmospheric scattering calculations.
    float3 boxStart = rayOrigin + intersectionDistances.x * rayDirection;
    float3 inScatterSamplePosition = boxStart;
    for (int i = 0; i < inScatterPoints; i++)
    {
        // Calculate the direction from the in-scatter point to the sun.
        float3 directionToSun = normalize(sunPosition - inScatterSamplePosition);
        
        // Perform a ray intersection from the sample position towards the sun.
        // This does not need a safety "is there any intersection at all?" check since by definition the sample position is already in the box, since it's an intersection
        // of a line in said box.
        float2 sunRayLengthDistances = GetRayBoxIntersectionOffsets(inScatterSamplePosition, directionToSun, boxMin, boxMax);
        float sunIntersectionRayLength = sunRayLengthDistances.y - sunRayLengthDistances.x;
        
        // Calculate the optical depth along the ray from the sample point to the sun.
        float sunIntersectionOpticalDepth = CalculateOpticalDepth(inScatterSamplePosition, directionToSun, sunIntersectionRayLength, sunIntersectionPoints);
        
        // Combine the two optical depths via exponential decay.
        float3 localScatteredLight = exp(-sunIntersectionOpticalDepth);
        
        // Combine the local scattered light, along with the density of the current position.
        light += CalculateCloudDensityAtPoint(inScatterSamplePosition) * float4(localScatteredLight, 1);
        
        // Move onto the next movement iteration by stepping forward on the in-scatter position.
        inScatterSamplePosition += rayDirection * inScatterStep;
    }
    
    // Perform Mie scattering on the result.
    float g = 0.67;
    float gSquared = g * g;
    float cosTheta = dot(rayDirection, normalize(sunPosition - boxStart));
    float cosThetaSquared = cosTheta * cosTheta;
    float phaseMie = ((1 - gSquared) * (cosThetaSquared + 1)) / (pow(1 + gSquared - cosTheta * g * 2, 1.5) * (gSquared + 2)) * 0.1193662; // This constant is equal to 3/(8pi)
    return light * inScatterStep * phaseMie * 320;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    position.xy = round(position.xy / 0.25) * 0.25;
    
    // Account for the pesky gravity potions...
    if (invertedGravity)
        position.y = screenSize.y - position.y;
    
    // Calculate how much scattered light will end up in the current fragment.
    float4 cloudLight = CalculateScatteredLight(float3(position.xy, -1), float3(0, 0, 1));
    cloudLight.rgb = 1 - exp(cloudLight.rgb * -cloudExposure);
    cloudLight *= lerp(4, 1, cloudDensity);
    
    float glowUVOffset = 1 + tex2D(baseTexture, coords * 0.8 + globalTime * 0.03) * 5.4;
    float2 aspectRatio = float2(screenSize.x / screenSize.y, 1);
    for (int i = 0; i < 10; i++)
    {
        float lightningIntensity = lightningIntensities[i];
        float distanceGlow = 0.15 / distance((coords - 0.5) * aspectRatio + 0.5, (lightningPositions[i] / screenSize - 0.5) * aspectRatio + 0.5);
        cloudLight *= clamp(1 + lightningIntensity * pow(1 - coords.y, 2.7 - lightningIntensity * 2.5) * distanceGlow * glowUVOffset.r, 1, 10);
    }
    
    // Combine the scattered light with the sample color, allowing for dynamic colorations and opacities to the final result.
    return saturate(cloudLight * sampleColor);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}