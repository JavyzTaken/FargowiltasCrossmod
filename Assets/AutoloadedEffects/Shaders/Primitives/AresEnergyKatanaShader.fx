sampler noiseTexture : register(s1);

bool flip;
float globalTime;
float appearanceInterpolant;
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
    
    // Pixelate everything.
    coords = round(coords * 75) / 75;
    
    float y = lerp(coords.y, 1 - coords.y, flip);
    
    float verticalRedThreshold = lerp(0.65, 0.95, pow(y, 0.6));
    float4 color = 1;
    
    // Bias colors from white to the sample color near the edges.
    color = lerp(color, input.Color, smoothstep(0.95, 0.99, coords.x / verticalRedThreshold));
    color = lerp(color, input.Color, smoothstep(0.25, 0.42, distance(coords.y, 0.5)));
    
    // Cut off the prims to create the front edge of the blade.
    clip(1 - coords.x / verticalRedThreshold);
    
    // Cut off the prims based on the appearance interpolant, to help make it look like the sword is being unsheathed at first.
    clip(tex2D(noiseTexture, coords * 0.75) * 0.25 + appearanceInterpolant - coords.x);
    
    return color * appearanceInterpolant;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
