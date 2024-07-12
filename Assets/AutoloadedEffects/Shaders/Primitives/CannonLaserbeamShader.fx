sampler noiseScrollTexture : register(s1);

float globalTime;
float2 laserDirection;
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
    
    float distanceFromCenter = distance(coords.y, 0.5);
    float distanceFromEdge = distance(distanceFromCenter, 0.4);
    
    float4 outerGlow = float4(0.98, 0.91, 0.75, 0) * saturate(pow(0.1 / distanceFromEdge, 3));
    float4 innerGlow = float4(0, 0.6, 0.6, 0) * tex2D(noiseScrollTexture, input.Position.xy / 33 - laserDirection * globalTime * 32);
    float4 glow = outerGlow + innerGlow;
    
    float opacity = smoothstep(0.55, 0.34, distanceFromCenter) * color.a;
    color += glow * color.a;
    
    return saturate(color) * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
