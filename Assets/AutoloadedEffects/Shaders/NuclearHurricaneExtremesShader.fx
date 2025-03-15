sampler baseTexture : register(s1);

bool top;
float localTime;
float4 color;
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
    float squish = QuadraticBump(coords.y) * 0.05;
    input.Position.x += squish * sign(input.TextureCoordinates.x - 0.5);
    input.Position.x += sin(localTime + (top ? -1 : 1) * 1.57) * 0.16;
    input.Position.x *= lerp(1.2, 0.45, top ? (1 - coords.y) : coords.y);
    
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

    float noise = tex2D(baseTexture, coords * float2(0.4, 1) + float2(localTime, 0));
    float whiteGlowNoise = pow(tex2D(baseTexture, coords * float2(0.51, 1) + float2(localTime * 0.9, 0)), 2);
    float opacity = QuadraticBump(coords.y) * smoothstep(0.15, 0.7, noise);
    
    return opacity * smoothstep(1, 0.7, abs(cosAngle)) * (color + smoothstep(0.5, 1, noise) * glowColor + whiteGlowNoise);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
