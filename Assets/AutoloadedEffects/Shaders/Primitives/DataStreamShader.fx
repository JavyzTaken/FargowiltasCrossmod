sampler streakHighlightTexture : register(s1);

float globalTime;
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
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float2 symbolsCoords = 1.25 - coords * float2(1, 1.5);
    symbolsCoords.x = frac(symbolsCoords.x + globalTime * 0.4);
    
    float symbols = tex2D(streakHighlightTexture, symbolsCoords);
    
    float horizontalDistanceFromCenter = distance(coords.y, 0.5);
    float4 color = input.Color * symbols;
    color += pow(0.5 - horizontalDistanceFromCenter, 2) * color.r * 2;
    
    return saturate(color) * pow(smoothstep(0, 0.03, coords.x), 2);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
