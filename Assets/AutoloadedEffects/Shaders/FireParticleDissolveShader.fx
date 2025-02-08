sampler baseTextureA : register(s1);
sampler baseTextureB : register(s2);
sampler noiseTexture : register(s3);

float pixelationLevel;
float turbulence;
float globalTime;
float initialGlowIntensity;
float initialGlowDuration;
float2 screenPosition;
float2 imageSize;
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
    // Scuffed way of sending per-particle data to the shader.
    float particleData = input.TextureCoordinates.z;
    float lifetimeRatio = saturate(frac(particleData));
    bool useAltTexture = particleData >= 1;
    
    float2 coords = input.TextureCoordinates.xy;
    float4 color = input.Color;
    
    // Apply pixelation to the fire.
    coords = round(coords * pixelationLevel) / pixelationLevel;

    // Oh dear.
    float frameIndex = floor(lerp(9, 36, lifetimeRatio));
    float4 frame = float4(frameIndex % 6, floor(frameIndex / 6), 0.16667, 0.16667) * imageSize.xyxy;
    float2 framedCoords = (coords * imageSize - frame.xy) / frame.zw;
    
    // Calculate noise. This shifts in position slightly based on lifetime ratio.
    float noise = tex2D(noiseTexture, lifetimeRatio * 0.15 + framedCoords);
    
    // Calculate the dissolve opacity.
    float dissolveThreshold = pow(lifetimeRatio, 1.5) * 1.1;
    float dissolveOpacity = smoothstep(-0.1, 0, noise - dissolveThreshold);
    
    // Apply distortion.
    coords += noise * lifetimeRatio * turbulence;
    
    // Apply brightening at the start of the particle's lifetime.
    color += smoothstep(initialGlowDuration, 0, lifetimeRatio) * color.a * initialGlowIntensity;
    color += tex2D(noiseTexture, framedCoords * 3) * float4(0.3, -0.05, 0, 0) * lifetimeRatio;
    
    float4 fireColor = lerp(tex2D(baseTextureA, coords), tex2D(baseTextureB, coords), useAltTexture);
    return fireColor * color * dissolveOpacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
