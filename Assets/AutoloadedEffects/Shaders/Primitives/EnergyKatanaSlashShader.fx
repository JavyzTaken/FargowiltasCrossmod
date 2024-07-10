sampler noiseTexture : register(s1);
sampler metalGlowTexture : register(s2);

bool clipTopHalf;
float globalTime;
float swingDirection;
float2 textureSize;
float2 slashSourceDirection;
float4 metalColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    clip(lerp(1 - coords.y, coords.y, clipTopHalf) - 0.5);
    
    float2 pixelationFactor = 2 / textureSize;
    coords = round(coords / pixelationFactor) * pixelationFactor;
    
    // Calculate polar coordinates.
    // This will be used numerously for noise texture calculations.
    float distanceFromCenter = sqrt(pow(coords.x - 0.5, 2) + pow(coords.y - 0.5, 2) * 1.1);
    float angleFromCenter = atan2(coords.y - 0.5, coords.x - 0.5);
    float2 polar = float2(distanceFromCenter, angleFromCenter / 3.141 + 0.5);
    
    // Calculate the blade end fade.
    // This ensures that the part of the blade that's far from the front fades away.
    float2 directionFromCenter = normalize(coords - 0.5);
    float slashDirectionOrthogonality = dot(directionFromCenter, slashSourceDirection);
    float bladeEndFadeoutValue = slashDirectionOrthogonality + distanceFromCenter * 0.9;
    float bladeEndFade = smoothstep(-0.56, 0.25, bladeEndFadeoutValue);
    
    // Calculate opacity values based on how close and far the pixel is from the center.
    float outerFade = smoothstep(0.5, 0.47, distanceFromCenter);
    float innerFade = pow(smoothstep(0.04, 0.24, distanceFromCenter), 3);
    
    // Combine the blend end fade and distance based opacity values.
    // Any values above 1 from this equation gets turned into an equivalent additive blending glow.
    float opacity = outerFade * innerFade * bladeEndFade * 2.15 - tex2D(noiseTexture, polar * float2(9, 1) + float2(0, globalTime * -swingDirection * 5));
    float slashGlow = clamp(opacity - 1, 0, 10);
    float4 slashColor = saturate(sampleColor + slashGlow) * opacity;
    
    // Calculate the color of the metal in the center which represents Ares' super-fast-moving arms.
    float metalReachNoise = pow(tex2D(noiseTexture, polar * float2(0.4, 1) + float2(0, globalTime * -swingDirection * 6)), 1.6) * 0.1;
    float metalColorInterpolant = smoothstep(0.3, 0.2, distanceFromCenter - metalReachNoise);
    float metalGlow = tex2D(metalGlowTexture, polar * float2(16, 1) + float2(0, globalTime * -swingDirection * 6)) * 0.4;
    float metalOpacity = smoothstep(0.03, 0.1, distanceFromCenter) * sqrt(bladeEndFade);
    
    // Combine colors.
    return lerp(slashColor, saturate(metalColor + metalGlow) * metalOpacity, metalColorInterpolant) * pow(sampleColor.a, 1.5);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}