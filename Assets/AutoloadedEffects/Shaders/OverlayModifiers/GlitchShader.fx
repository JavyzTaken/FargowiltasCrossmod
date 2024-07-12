sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float time;
float2 textureSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelationFactor = 2 / textureSize;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float glitchStatic = (tex2D(noiseTexture, coords * 20 + time * 60) + tex2D(noiseTexture, coords * 32 + time * -19) + tex2D(noiseTexture, coords * 16 + time * -9)) * 0.333;
    
    float glitchPixelationFactor = 5;
    float timeStepFactor = 11;
    float glitchCoords = floor(coords.y * glitchPixelationFactor) / glitchPixelationFactor + floor(time * timeStepFactor) / timeStepFactor;    
    float glitchNoise = tex2D(noiseTexture, glitchCoords);
    
    glitchCoords = floor(coords.y * glitchPixelationFactor * 0.5) / glitchPixelationFactor + floor(time * timeStepFactor * 1.5) / timeStepFactor;
    glitchNoise = (glitchNoise + tex2D(noiseTexture, glitchCoords)) * 0.67;
    
    glitchCoords = floor(coords.y * glitchPixelationFactor * 0.4) / glitchPixelationFactor + floor(time * timeStepFactor * 1.3) / timeStepFactor;
    glitchNoise = (glitchNoise + tex2D(noiseTexture, glitchCoords)) * 0.62;
    
    coords.x += smoothstep(0.5, 1, glitchNoise) * 0.7;
    coords = floor(coords / pixelationFactor) * pixelationFactor;
    
    float4 color = tex2D(baseTexture, coords);
    color += glitchStatic * color.a * 0.4;
    
    return color * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}