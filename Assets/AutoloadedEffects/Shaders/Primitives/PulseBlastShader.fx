sampler noiseTexture : register(s1);

float globalTime;
float lengthRatios[10];
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
    
    // Determine how far the laser should go.
    float horizontalIndexValue = coords.y * 9.99;
    float index = floor(horizontalIndexValue);
    float maxLength = lerp(lengthRatios[index], lengthRatios[index + 1], frac(horizontalIndexValue));
    
    // Calculate glow values.
    float horizontalDistanceFromCenter = distance(coords.y, 0.5);
    float glowIntensity = lerp(0.03, 0.06, tex2D(noiseTexture, coords * float2(6, 1) - float2(globalTime * 6, 0)) );    
    float innerHorizontalGlow = glowIntensity / horizontalDistanceFromCenter;
    float endGlow = smoothstep(0.8, 0.9, coords.x);
    
    // Apply glow to the result.
    color = saturate(color + innerHorizontalGlow + endGlow);
    
    // Opacity hell.
    return color * smoothstep(0.5, 0.35, horizontalDistanceFromCenter) * smoothstep(1, 0.95, coords.x / maxLength) * (coords.y < 1) * smoothstep(0, 0.04, coords.x);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
