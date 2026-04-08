#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float4x4 viewWorldProjection;
float time;
float4 shaderData;
float3 color;


struct VertexShaderInput
{
    float4 pos : POSITION0;
    float4 col : COLOR0;
    float2 texCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 texCoord : TEXCOORD0;
};
float sinBetween(float a, float b, float v)
{
    float h = (b - a) / 2.;
    return a + h + sin
    (v) * h;
}
float2 turbulence(float2 p)
{
    float freq = 7.;
    
    float2x2 rot = float2x2(0.6, -0.4, 0.4, 0.6);
    
    for (float i = 0.0; i < 15; i++)
    {
        float phase = freq * mul(p, rot).y + 2 * time + i;
        p += -1 * rot[0] * cos(phase) / freq;
        
        rot *= float2x2(0.6, -0.8, 0.8, 0.6);
        freq *= 1.75;
    }
    
    return p;
}
float3 palette(float t)
{
    float3 a = float3(-0.462, 3.078, 0.878);
    float3 b = float3(1.564, 2.450, -0.112);
    float3 c = float3(1.860
    , 1.208, -6.142);
    float3 d = float3(6.285
    , 6.285
    , 6.813);

    return a + b * cos(6.28318 * (c * t + d));
}
float4 ShaderPS(float4 vertexColor : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
    float2 centeredUV = texCoords;
    
    centeredUV *= 2;
    centeredUV -= 1;
    
    float2 distanceUV = 1 / distance(float2(0.5, 0.5), (centeredUV / 30));
      
    distanceUV *= smoothstep(1, 0, length(texCoords * 2 - 1) * 1);
    float pulse = sin(time * 45) * 0.3 + 1;
    float3 col = palette(centeredUV.x / 2 + centeredUV.y / 4) * float3(1, 0.3, 1);
    float2 bolt = (distanceUV * pulse);
    float3 highestRGBValue;
    if (col.r > col.b)
        highestRGBValue = float3(1, 0, 0);
    else if (col.b > col.g)
        highestRGBValue = float3(0,1,0);
    else
        highestRGBValue = float3(0,0,1);
    
    return bolt.x * float4(lerp(highestRGBValue, col
, bolt.x),
    bolt.x);

}

technique t0
{
    pass DukeWaterStream
    {
        PixelShader = compile ps_3_0 ShaderPS();
    }
}