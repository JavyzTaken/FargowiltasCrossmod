sampler noiseTexture : register(s1);
sampler flamelashTexture : register(s2);

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
    float4 color = input.Color;
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float dissolveNoise = tex2D(noiseTexture, coords * float2(2.2, 1) + float2(-globalTime * 3, 0));
    float dissolve = smoothstep(-0.15, 0.1, dissolveNoise - coords.x * 1.3 + 0.2);
    
    color *= smoothstep(0.4 - coords.x * 0.5, 1, QuadraticBump(coords.y));
    color += smoothstep(0.35, 0, coords.x) * color.a;
    
    color += tex2D(flamelashTexture, coords + float2(globalTime * -1.9, dissolveNoise * 0.05)).r;
    
    return color * dissolve;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}