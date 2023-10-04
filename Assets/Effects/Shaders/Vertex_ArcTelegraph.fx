sampler noiseImage : register(s1);
float time;
float3 mainColor;

matrix worldViewProjection;

struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, worldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
};

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
    float bloomOpacity = pow(cos(coords.x * 4.8 - 0.8), 55 + pow(coords.y, 4) * 700);
    
    // Create some noisy opaque blotches in the inner part of the trail.
    if (coords.x > 0.15 && coords.x < 0.85)
    {
        float noise = tex2D(noiseImage, coords * 0.8 + float2(0, time * -0.6)).r;
        float noise2 = tex2D(noiseImage, coords * 0.5 + float2(0, time * -0.4)).r;
        float finalNoise = noise * 0.5 + noise2 + 0.5;
        float minOpacity = pow(1 - sin(coords.x * 3.141) + finalNoise * 2.2, 0.2);
        bloomOpacity += lerp(0.04, 0.8, minOpacity);
    }
    
    float opacity = 1;
    if (coords.y > 0.9)
        bloomOpacity *= pow(1 - (coords.y - 0.9) / 0.1, 1);
    else if (coords.y < 0.1)
        bloomOpacity *= pow(coords.y / 0.1, 1);

    return lerp(color, float4(mainColor, color.a), coords.y) * bloomOpacity * 0.6;
    
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();

    }
}