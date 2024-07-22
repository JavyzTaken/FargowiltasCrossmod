sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float globalTime;
float dissolveInterpolant;
float2 dissolveCenter;
float2 screenPosition;
float2 textureSize0;
float2 dissolveDirection;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 screenUV = (position.xy + screenPosition) / textureSize0;
    
    // Offset pixel in accordance with the dissolve direction and interpolant, making them effectively disintegrate as the distortion becomes more and more intense.
    float dissolveDirectionNoise = pow(tex2D(noiseTexture, screenUV * float2(14, 0)) * tex2D(noiseTexture, screenUV * float2(23, 0.04)), 0.75);
    float2 dissolveOffset = dissolveDirection * dissolveDirectionNoise * dissolveInterpolant * 2.4;
    float2 pixelationFactor = 4 / textureSize0;
    dissolveOffset = round(dissolveOffset / pixelationFactor) * pixelationFactor;    
    coords += dissolveOffset;
    
    // Apply a secondary, distance-from-center based disintegration effect as well.
    float dissolveNoise = (tex2D(noiseTexture, screenUV * textureSize0 * 0.002) + tex2D(noiseTexture, screenUV * textureSize0 * 0.0032)) * 0.5;
    clip(dissolveNoise - dissolveInterpolant - distance(coords, dissolveCenter) + 0.15);
    
    // Sample colors in such a manner that's analogous to a silhouette, only drawing a single color.
    float4 color = tex2D(baseTexture, coords);
    return any(color) * sampleColor * color.a;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}