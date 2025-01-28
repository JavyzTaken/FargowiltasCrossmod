sampler baseTexture : register(s0);

float blurInterpolant;
float blurWeights[12];
float2 blurDirection;
float2 disappearPoint;
float2 originDirection;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 offsetFromOrigin = disappearPoint - position.xy;
    float signedDistanceFromOrigin = dot(originDirection, offsetFromOrigin);
    float opacity = smoothstep(0, -120, signedDistanceFromOrigin);
    
    float4 blurredColor = 0;
    float2 blurOffset = blurDirection * blurInterpolant * 0.0015;
    for (int i = 0; i < 12; i++)
    {
        float blurWeight = blurWeights[i] * 0.5;
        blurredColor += tex2D(baseTexture, coords - i * blurOffset) * blurWeight;
        blurredColor += tex2D(baseTexture, coords + i * blurOffset) * blurWeight;
    }
    
    float4 color = lerp(tex2D(baseTexture, coords), blurredColor * 0.7, sqrt(blurInterpolant)) * sampleColor;    
    return color * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}