sampler baseTexture : register(s0);
sampler electricityNoiseTexture : register(s1);
sampler distanceNoiseTexture : register(s2);

float globalTime;
float2 textureSize0;

float CalculateOuterGlowIntensity(float2 coords, float distanceFromCenter)
{
    // Rewrite the outer glow intensity to be more explicitly defined in terms of the edge.
    // This evaluates how far away the distance from the center is relative to a given radius.
    // If the distance from the center is 0.24, for example, then this evaluates to 0.01, since it's 0.01 units away from 0.25.
    // This makes the results feel less foggy than they were before.
    float edgeBrightness = 0.05;
    float distanceFromEdge = distance(distanceFromCenter, 0.25);
    return edgeBrightness / distanceFromEdge * smoothstep(0.4, 0.25, distanceFromCenter);
}

float CalculateInnerGlowIntensity(float2 coords, float distanceFromCenter)
{
    // Apply the same techniques as with the outer glow, to create an inner circle of light.
    float sphereBrightness = 0.36;
    
    float glowIntensity = sphereBrightness / distanceFromCenter;
    glowIntensity *= smoothstep(0.08, 0, distanceFromCenter);
    
    return glowIntensity;
}

float CalculateElectricityIntensity(float2 coords, float distanceFromCenter)
{
    // The old electricity scroll effect looked really static and boring.
    // Instead, use polar coordinates relative to the center of the circle.
    // This allows for scrolling to happen based on distance and angle, rather than X/Y position, allowing for an "outflow" effect.
    float2 polar = float2(distanceFromCenter, atan2(coords.y - 0.5, coords.x - 0.5) / 6.283 + 0.5);
    
    // Make the angle part of the polar coordinates loop 2 times across the circle instead of 1, giving a more line-y look.
    // 4 was a bit much with the effect below, hence its reduction.
    // Update -- So was 3. It has been reduced to 2.
    polar.y *= 2;
    
    // Use two noise values instead of one, with differing fast scrolls.
    float noiseA = tex2D(electricityNoiseTexture, polar + float2(globalTime * -1.9, 0) - float2(0, polar.x));
    float noiseB = tex2D(electricityNoiseTexture, polar * float2(0.5, 2) + globalTime * float2(-1.2, 0.1) + float2(0, polar.x));
    float electricityIntensity = (noiseA + noiseB) * 0.6;
    
    // Use a 3x^5 polynomial on the intensity.
    // Since the electricity intensity is expected to be in a 0-1 range, the x^5 term will bias it HEAVILY towards 0, making it so that only the brightest parts of the electricity can be present.
    // The multiplication by 3 counteracts this somewhat, allowing for the few bits of electricity that aren't biased by the power function to become white in a much "sharper" manner.
    electricityIntensity = pow(electricityIntensity, 5) * 3;
    
    // Ensure that the electricity effect tapers off by distance, to ensure that it doesn't appear beyond the edge of the circle.
    electricityIntensity *= smoothstep(0.3, 0.2, distanceFromCenter);

    return electricityIntensity;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the distance from the center at the start, rather than multiple times in function areas, and apply a bit of noise to it to make the successive calculations a bit more rough, and not as
    // mathematically perfect. This only affects the distance calculations by a maximum of 0.02 (where the overall circle has a radius of 0.5), ensuring that the effect is noticeable but subtle.
    // This also uses the same polar calculation from before, to ensure that the distance offset travels in an outflow manner.
    float distanceFromCenter = distance(coords, 0.5);
    float2 polar = float2(distanceFromCenter, atan2(coords.y - 0.5, coords.x - 0.5) / 6.283 + 0.5);
    float distanceOffset = tex2D(distanceNoiseTexture, polar * float2(3, 4) + globalTime * float2(-2.6, 0)).x * 0.02;
    distanceFromCenter += distanceOffset;
    
    // Combine the two base glow intensity values by adding them.
    // Since they both occupy different parts in the rendering process (the outer edge versus the inner core), this addition will not interfere
    // with either calculation, since only one of the two terms should ever be above zero.
    float outerGlowIntensity = CalculateOuterGlowIntensity(coords, distanceFromCenter);
    float innerGlowIntensity = CalculateInnerGlowIntensity(coords, distanceFromCenter);
    
    float electricityIntensity = CalculateElectricityIntensity(coords, distanceFromCenter);
    
    // Separate the combined glow intensity calculation into a glow color calculation.
    // This allows for the application of the sampleColor tint to the outer glow exclusively, rather than to overall combined result, allowing the inner glow to stay white-tinted for now.
    float4 glowColor = outerGlowIntensity * sampleColor + innerGlowIntensity + electricityIntensity;
    
    // Apply two post-processing steps to the overall circle:
    // 1. Bias the colors towards pure blue (making red and green colors, and consequently cyan and white) less pronounced the closer they are to the center. This effect naturally weakens the further from the center we are.
    //    The factor of 0.2 ensures this effect is relatively subtle.
    // 2. Bias the colors towards cyan blue (aka blue but also green) if they're really close to the center. This helps to give the electricity streaks from the CalculateElectricityIntensity function a cyan edge to them as they
    //    emerge from the core of this electric circle. This effect is more pronounced than the one above, due to its factor of 0.7.
    glowColor += float4(-1, -1, 1, 0) * smoothstep(0.4, 0, distanceFromCenter) * 0.2;
    glowColor += float4(-1, -0.15, 1, 0) * smoothstep(0.2, 0, distanceFromCenter) * 0.7;
    
    // Make the translucent parts of the texture blue.
    // This effect naturally fades off based on distance, so that it doesn't appear beyond the edge of the circle.
    // UPDATE -- This effect has been made stronger, to make the colors a bit less pastel.
    glowColor += float4(0, 0.45, 1, 1) * (1 - glowColor.a) * smoothstep(0.33, 0.2, distanceFromCenter) * 0.9;
    
    return glowColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}