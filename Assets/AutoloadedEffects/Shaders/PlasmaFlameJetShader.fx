sampler gradientMapTexture : register(s1);

float localTime;
float glowPower;
float edgeFadeThreshold;
float4 glowColor;
matrix uWorldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Calculate a base scrolling noise value.
    // This will be used for distorting future noise calculations and for the purpose of calculating the glow of this pixel.
    float noise = tex2D(gradientMapTexture, coords * float2(2.93, 0.6) + float2(localTime * -3.5, 0.54));
    
    // Calculate the cutoff offset noise.
    // The results of this will be used to determine how the pixel fades away.
    // It depends on the following attributes:
    // 1. Horizontal distance from the center of the laser. This results in edge-fading effects.
    // 2. A noise calculation, to help make the results look more pseudo-random, like a flame would.
    // 3. Vertical distance from the center of the laser, at the start and end.
    //    This ensures that the results naturally dissipate at the end points, rather than having an unnatural, flat cutoff.
    float horizontalDistanceFromCenter = distance(coords.y, 0.5);
    float cutoffValue = horizontalDistanceFromCenter;
    cutoffValue += tex2D(gradientMapTexture, coords * float2(4.1, 0.854) + float2(localTime * -5.9 - noise * 0.4, 0) - noise * 0.1) * 0.6;
    cutoffValue += smoothstep(0.1, 0, coords.x) * 0.6;
    cutoffValue += smoothstep(0.8, 1, coords.x);
    
    // Use the above value to calculate the cutoff interpolant.
    float cutoffOpacity = smoothstep(0.5, 0.5 - edgeFadeThreshold, cutoffValue);
    
    // Calculate the glow intensity value. This will dictate how bright the pixel is.
    float glow = saturate(noise - horizontalDistanceFromCenter) + pow(0.1 / horizontalDistanceFromCenter, glowPower);
    
    return saturate(color + glowColor * glow) * cutoffOpacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
