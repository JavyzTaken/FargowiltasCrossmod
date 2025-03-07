sampler baseTexture : register(s1);

float localTime;
float maxBumpSquish;
float wavinessFactor;
float dissolveInterpolant;
float zoom;
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
    float4 color = input.Color;
    float cosAngle = coords.x;
    coords.x = acos(cosAngle) / 3.141;
    
    // Make the top and sides of the hurricane dissipate.
    float opacity = smoothstep(0.1, 0.5, QuadraticBump(coords.y)) * smoothstep(1, 0.95, abs(cosAngle));
    
    // Calculate an initial noise value.
    float noise = tex2D(baseTexture, coords * float2(1, 2) + float2(localTime * 0.5, coords.x * 0.5));
    
    // Use the first noise value as a warping offset on a second noise value, which will dictate the appearance of foam on the hurricane.
    noise = tex2D(baseTexture, (coords * float2(1, 2) + float2(localTime * 1.3, coords.x * 1.7) + noise * 0.02) * zoom);

    return smoothstep(0.45, 1, noise - dissolveInterpolant * 0.5) * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
