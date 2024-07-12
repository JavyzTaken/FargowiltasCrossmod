sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float time;
float2 textureSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelationFactor = 2 / textureSize;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    // Calculate values for glitch noise.
    // This will be used to offset the texture in bar-like formations.
    float glitchPixelationFactor = 5;
    float timeStepFactor = 11;
    float glitchCoords = floor(coords.y * glitchPixelationFactor) / glitchPixelationFactor + floor(time * timeStepFactor) / timeStepFactor;    
    float glitchNoise = tex2D(noiseTexture, glitchCoords);
    
    glitchCoords = floor(coords.y * glitchPixelationFactor * 0.5) / glitchPixelationFactor + floor(time * timeStepFactor * 1.5) / timeStepFactor;
    glitchNoise = (glitchNoise + tex2D(noiseTexture, glitchCoords)) * 0.67;
    
    glitchCoords = floor(coords.y * glitchPixelationFactor * 0.4) / glitchPixelationFactor + floor(time * timeStepFactor * 1.3) / timeStepFactor;
    glitchNoise = (glitchNoise + tex2D(noiseTexture, glitchCoords)) * 0.62;
    
    // Perform the glitch offset and re-pixelate.
    coords.x += smoothstep(0.5, 1, glitchNoise) * 0.7;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float4 color = tex2D(baseTexture, coords);
    
    // Calculate a static glitch noise value.
    // This will be used to accent the texture a bit and give a perception of being a hologram.
    float glitchStatic = (tex2D(noiseTexture, coords * 20 + time * 60) + tex2D(noiseTexture, coords * 32 + time * -19) + tex2D(noiseTexture, coords * 16 + time * -9)) * 0.333;
    color += glitchStatic * color.a * 0.4;
    
    // Make the bottom of the sprite fade, to help sell the idea that the results aren't fully material.
    float bottomFade = pow(smoothstep(1, 0.45, coords.y), 0.4);
    
    return color * sampleColor * bottomFade;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}