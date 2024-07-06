sampler streakNoiseTexture : register(s1);

float globalTime;
bool verticalFlip;
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
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.TextureCoordinates.y = (input.TextureCoordinates.y - 0.5) / input.TextureCoordinates.z + 0.5;

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
    
    // Flip the coords as necessary.
    coords.y = lerp(coords.y, 1 - coords.y, verticalFlip);
    
    // Bias colors towards red.
    color.gb -= coords.y * coords.x * 0.8;
    
    // Determine whether the color needs to be erased based on the streak.
    float streak = tex2D(streakNoiseTexture, coords * 1.5 + float2(globalTime * -3.3, 0));
    bool erasePixel = streak >= 1.2 - coords.x * 0.6 - coords.y;
    
    // Calculate the brightness interpolant.
    float brightnessInterpolant = 1 - smoothstep(coords.x + streak * 0.09, 0, 0.32);
    
    return (color + brightnessInterpolant * color.a * 1.86) * (1 - erasePixel);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
