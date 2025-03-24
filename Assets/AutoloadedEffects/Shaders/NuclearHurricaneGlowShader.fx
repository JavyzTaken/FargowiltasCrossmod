sampler baseTexture : register(s1);

float localTime;
float maxBumpSquish;
float wavinessFactor;
float4 glowColor;
matrix uWorldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float QuadraticBump(float x)
{
    return x * (4 - x * 4);
}

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    
    float2 coords = input.TextureCoordinates;
    float bump = QuadraticBump(coords.y);
    input.Position.x += sin(coords.y * 6.283 + localTime) * bump * wavinessFactor;
    input.Position.x *= lerp(1, maxBumpSquish, bump);
    
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float cosAngle = coords.x;
    coords.x = acos(cosAngle) / 3.141;
    
    float opacity = smoothstep(1, 0.1, abs(cosAngle));
    float extremesFade = smoothstep(0.2, 1, QuadraticBump(coords.y));
    return glowColor * opacity * extremesFade;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
