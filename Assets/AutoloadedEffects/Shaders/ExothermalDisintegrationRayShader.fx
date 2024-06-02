sampler noiseScrollTexture : register(s1);
sampler lightningScrollTexture : register(s2);

float globalTime;
float edgeGlowIntensity;
float2 laserDirection;
float3 edgeColorSubtraction;
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
    float4 color = input.Color;
    
    // Account for texture distortion artifacts in accordance with the primitive distortion fixes.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Calculate the edge glow, creating a strong, bright center coloration.
    float distanceFromCenter = distance(coords.y, 0.5);
    float edgeGlow = edgeGlowIntensity / pow(distanceFromCenter, 0.8);
    color = saturate(color * edgeGlow);
    
    // Apply subtractive blending that gets stronger near the edges of the beam, to help with saturating the colors a bit.
    color.rgb -= distanceFromCenter * edgeColorSubtraction;
    
    // Apply additive blending in accordance with a lightning scroll texture.
    color += tex2D(lightningScrollTexture, coords * float2(0.9, 1) + float2(globalTime * -1.9, 0)).r * color.a * (color.g + 0.35);
    
    // Apply a glow to the horizontal center.
    color += 0.1 / distanceFromCenter;
    
    // Fade at the edges.
    color *= smoothstep(0.5, 0.3, distanceFromCenter);
    
    // Fade at the laser's end.
    float noise = tex2D(noiseScrollTexture, coords * float2(0.8, 1.75) + float2(globalTime * -3.3, 0));
    float endOfLaserFade = smoothstep(0.98, 0.9 + noise * 0.06, coords.x);
    color *= endOfLaserFade;
    
    // Apply some fast, scrolling noise to the overall result.
    return color * (noise + 1 + step(0.5, noise + (0.5 - distanceFromCenter)));
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
