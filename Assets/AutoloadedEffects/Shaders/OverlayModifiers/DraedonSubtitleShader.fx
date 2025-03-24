sampler baseTexture : register(s0);

float globalTime;
float pixelation;
float2 textureSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 pixelationFactor = pixelation / textureSize;
    coords = round(coords / pixelationFactor) * pixelationFactor;
    
    float4 baseColor = tex2D(baseTexture, coords);
    baseColor.rgb *= lerp(0.7, 1.4, cos(coords.x * 40 + globalTime * -8) * 0.5 + 0.5);
    
    return baseColor * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}