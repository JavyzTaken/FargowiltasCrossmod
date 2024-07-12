sampler baseTexture : register(s0);

bool cutOffAtTop;
bool invertCutoff;
float globalTime;
float2 lightPosition;
float2 textureSize0;
float4 frame;
float4 lightColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 screenPosition : SV_POSITION) : COLOR0
{
    float2 framedCoords = (coords * textureSize0 - frame.xy) / frame.zw;
    framedCoords.y = lerp(framedCoords.y, 1 - framedCoords.y, invertCutoff);
    
    float4 baseColor = tex2D(baseTexture, coords);
    float2 lightDirection = normalize(lightPosition - screenPosition.xy) * float2(-1, 1);
    
    // Take a blurred sample of the texture and create normals based off of its neighbors in a manner analogous to a derivative.
    float2 normal = 0;
    float2 textureOffset = 4 / textureSize0;
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            float2 blurredCoords = coords + float2(i, j) * 2 / textureSize0;
            float baseBrightness = tex2D(baseTexture, blurredCoords).x;
            float rightBrightness = tex2D(baseTexture, blurredCoords + float2(textureOffset.x, 0)).x;
            float bottomBrightness = tex2D(baseTexture, blurredCoords + float2(0, textureOffset.y)).x;
            normal += (baseBrightness - float2(rightBrightness, rightBrightness)) * 0.4;
        }
    }
    
    // Calculate light based on the normal vector, along with cut-off calculations.
    float light = saturate(dot(lightDirection, normal) + (smoothstep(0.3, 0.1, framedCoords.y) - smoothstep(0.45, 0.35, framedCoords.y) * 1.5) * cutOffAtTop);
    
    // Combine the lighting together with the base color.
    baseColor *= sampleColor;
    return baseColor + lightColor * pow(light, 1.2) * baseColor.a;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}