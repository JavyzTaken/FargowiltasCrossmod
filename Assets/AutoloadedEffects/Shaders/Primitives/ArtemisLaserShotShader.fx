sampler streakHighlightTexture : register(s1);

float globalTime;
float glowIntensity;
float noiseScrollOffset;
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

float QuadraticBump(float x)
{
    return x * (4 - x * 4);
}

float4 Posterize(float4 color)
{
    float gamma = 0.96;
    color = pow(color, gamma);
    color = floor(color * 6) / 6;
    color = pow(color, 1.0 / gamma);
    return color;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float yellowInterpolant = saturate(QuadraticBump(coords.y) - coords.x * 1.2);
    float whiteInterpolant = saturate(pow(QuadraticBump(coords.y), 1.5) - lerp(-0.1, 1, coords.x) * 2.5);
    
    float4 baseResult = color + float4(1, 1, 0, 0) * yellowInterpolant + whiteInterpolant * 2;
    float4 posterizedResult = Posterize(baseResult);
    
    return posterizedResult;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
