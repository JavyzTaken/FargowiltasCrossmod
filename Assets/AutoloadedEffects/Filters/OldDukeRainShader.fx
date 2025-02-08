sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);

float time;
float opacity;
float rainOpacity;
float rainAngle;
float2 zoom;
float2 screenCoordsOffset;
float4 rainColor;

float2 RotatedBy(float2 v, float theta)
{
    float s = sin(theta);
    float c = cos(theta);
    return float2(v.x * c - v.y * s, v.x * s + v.y * c);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 rainCoords = RotatedBy(coords + screenCoordsOffset * zoom - 0.5, rainAngle) * float2(0.8, 8.5) / zoom + 0.5;
    float rain = tex2D(noiseTexture, rainCoords + float2(time * 1.5, 0)) +
                 tex2D(noiseTexture, rainCoords * float2(1.5, 6) + float2(time * 1.7, 0)) +
                 tex2D(noiseTexture, rainCoords * float2(0.5, 1.5) + float2(time * 2.3, 0));
    
    rain = pow(smoothstep(0, 0.7, rain * 0.5), 2) * rainOpacity;
    
    return tex2D(baseTexture, coords) + rain / (4 - rainColor) * rainColor * pow(opacity, 3);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
