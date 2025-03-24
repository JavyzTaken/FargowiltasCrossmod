sampler noiseTextureA : register(s1);
sampler noiseTextureB : register(s2);

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

float QuadraticBump(float x)
{
    return x * (4 - x * 4);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Calculate white-hot glowy effects based on noise.
    // This is strongest at the horizontal center of the effect.
    float whiteHotNoise = 1 - tex2D(noiseTextureA, coords + float2(globalTime * -4.3, 0));
    float whiteHotBrightness = pow(QuadraticBump(coords.y) * (1 - coords.x), 1.3) / whiteHotNoise;
    
    float edgeOpacity = QuadraticBump(coords.y);
    float4 color = input.Color * edgeOpacity * 1.6;
    
    // Apply noise-based color accents by biasing the base green colors towards cyans.
    color -= float4(0.81, 0.31, -0.208, 0) * tex2D(noiseTextureA, coords * float2(0.7, 1.6) + float2(globalTime * -3.3, 0.16)) * color.a;
    
    // Ensure that the brightest part of the flame engulf effect is at the tip, with other parts being significantly faded in comparison.
    color = clamp(color, 0, 10) * (1 - QuadraticBump(coords.y) * smoothstep(0, 0.18, coords.x));
    
    return color + whiteHotBrightness * color.a;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
