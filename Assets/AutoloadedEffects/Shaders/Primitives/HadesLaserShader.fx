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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float distanceOffset = (tex2D(streakHighlightTexture, coords + float2(globalTime * -2.8 + noiseScrollOffset, 0)) - 0.5) * 0.156;
    
    float distanceFromCenter = distance(coords.y, 0.5) + distanceOffset;
    float glow = pow(0.25 / distanceFromCenter, 2.5) * QuadraticBump(coords.y) * glowIntensity;
    
    // This is done to ensure that when multiplying all color channels receive a boost. If color is rgb(1, 0, 0) then it's impossible for Green and Blue channels to increase to create white.
    color += glow * 0.001;
    
    return color * glow;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
