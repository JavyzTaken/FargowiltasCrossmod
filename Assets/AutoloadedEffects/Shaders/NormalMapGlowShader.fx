sampler baseTexture : register(s0);
sampler normalMapTexture : register(s1);

float globalTime;
float glowIntensity;
float3 lightColor;
float2 lightSourcePosition;
float2 textureSize1;

float3 SampleNormal(float2 coords)
{
    float2 offset = 2 / textureSize1;
    float3 left = tex2D(normalMapTexture, coords + float2(-1, 0) * offset).xyz;
    float3 right = tex2D(normalMapTexture, coords + float2(1, 0) * offset).xyz;
    float3 top = tex2D(normalMapTexture, coords + float2(0, -1) * offset).xyz;
    float3 bottom = tex2D(normalMapTexture, coords + float2(0, 1) * offset).xyz;
    float3 center = tex2D(normalMapTexture, coords).xyz;
    
    return (left + right + top + bottom + center) * 0.2;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 screenPosition : SV_POSITION) : COLOR0
{
    float4 baseColor = tex2D(baseTexture, coords) * sampleColor;
    float3 normal = normalize((SampleNormal(coords) * 2 - 1) * float3(1, -0.75, 1) + float3(0, 0, -0.35));
    float3 directionToLight = normalize(float3(lightSourcePosition, 0) - float3(screenPosition.xy, 0));
    float brightness = saturate(dot(normal, directionToLight));
    brightness = pow(brightness, 1.8);
    
    return baseColor + float4(lightColor, 0) * brightness * baseColor.a * glowIntensity;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}