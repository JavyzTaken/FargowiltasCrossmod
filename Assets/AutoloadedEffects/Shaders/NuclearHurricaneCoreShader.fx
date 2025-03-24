sampler baseTexture : register(s1);

float localTime;
float maxBumpSquish;
float wavinessFactor;
float opacity;
float dissolveInterpolant;
float3 baseColor;
float3 additiveAccentColor;
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
    
    // Calculate scrolling noise along the cylinder.
    // The X coordinate is offset by the Y coordinate to make it look like the hurricane is constantly scrolling upright.
    float bump = QuadraticBump(coords.y);
    float wave = sin(coords.y * 6.283 + localTime) * 0.2;
    float noise = tex2D(baseTexture, coords * 0.3 + float2(coords.y * wave, -localTime));
    float3 color = baseColor * lerp(0.5, 1, noise);
    
    // Apply additive blending.
    float additiveNoise = tex2D(baseTexture, (coords + float2(coords.y * wave + noise * 0.5, -localTime)) * 0.85);
    color += additiveNoise * additiveAccentColor;
    
    // Make the horizontal edges of the hurricane fade out to make the effect look more like a fluid of sorts.
    float horizontalEdgeFade = smoothstep(1, 0.85, abs(cosAngle));
    
    // Make the top and bottom fade out. A separate effect will handle that.
    float extremesFade = smoothstep(0.2, 1, bump);
    
    float dissolveNoise = tex2D(baseTexture, (coords * float2(2, 1) + float2(coords.y * wave + noise * 0.4, -localTime)) * 1.2);
    float dissolveOpacity = smoothstep(-0.2, 0, dissolveNoise - dissolveInterpolant * 1.2);
    
    return float4(saturate(color), 1) * horizontalEdgeFade * opacity * extremesFade * dissolveOpacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
