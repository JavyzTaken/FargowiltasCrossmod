sampler scrollTextureA : register(s1);
sampler scrollTextureB : register(s2);

float globalTime;
float3 scrollColorA;
float3 scrollColorB;
float3 baseColor;
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
    output.TextureCoordinates = (output.TextureCoordinates - 0.5) / input.TextureCoordinates.z + 0.5;

    return output;
}

float3 HSVToRGB(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    float scrollTime = globalTime * 1.2;
    float baseColorInterpolant = smoothstep(0.5, 0.42, tex2D(scrollTextureB, coords * float2(1, 1.2) + scrollTime * float2(-3, 0)));
    baseColorInterpolant += smoothstep(0.08, 0.01, coords.x);
    
    color = float4(HSVToRGB(float3(coords.x * 3 + baseColorInterpolant * 0.3 - scrollTime * 1.3, 1, 1)) + 0.5, 1);
    color = lerp(color, float4(baseColor, 1), saturate(baseColorInterpolant));
    
    return color;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
