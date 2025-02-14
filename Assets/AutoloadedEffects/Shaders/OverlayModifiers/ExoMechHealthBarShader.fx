sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float globalTime;
float horizontalSquish;
float2 imageSize;
float4 sourceRectangle;

float gradientCount;
float3 gradient[8];

float3 PaletteLerp(float interpolant)
{
    int startIndex = clamp(interpolant * gradientCount, 0, gradientCount - 1);
    int endIndex = clamp(startIndex + 1, 0, gradientCount - 1);
    return lerp(gradient[startIndex], gradient[endIndex], frac(interpolant * gradientCount));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    // Calculate UV coordinates relative to the squished health bar frame.
    float2 framedCoords = (coords * imageSize - sourceRectangle.xy) / sourceRectangle.zw;
    framedCoords.x *= horizontalSquish;
    
    // Calculate the hue interpolant. This will dictate the choice of color on the Exo palette gradient.
    // The inclusion of the Y position helps to make the effect slightly slanted, making the colors more interesting than just a simple gradient.
    float hueInterpolant = cos(globalTime * 2 - framedCoords.x * 3.141 + coords.y * -10) * 0.5 + 0.5;
    float4 color = float4(PaletteLerp(hueInterpolant), 1);
    
    // Combine three scrolling noise values that dictate which parts of the gradient get biased towards white.
    float smallFastScrollNoise = 1 - tex2D(noiseTexture, framedCoords * float2(0, 0.04) + float2(globalTime * -0.4, 0));
    float midNoise = 1 - tex2D(noiseTexture, framedCoords * float2(0, 0.06) + float2(globalTime * -0.3, 0));
    float bigSlowScrollNoise = 1 - tex2D(noiseTexture, framedCoords * float2(0, 0.17) + float2(globalTime * -0.2, 0));
    float noise = smallFastScrollNoise + midNoise + bigSlowScrollNoise;
    
    // Bias colors towards white based on the noise.
    color = lerp(color, 1, saturate(noise) * 0.6);
    
    // Make the white notch at the edge of the bar white.
    float notchInterpolant = smoothstep(0.995 - 0.01 / horizontalSquish, 0.995, framedCoords.x / horizontalSquish);
    color.rgb += pow(notchInterpolant, 2);
    
    return color * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}