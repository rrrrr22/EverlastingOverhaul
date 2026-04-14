#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
sampler2D spritebatchImage : register(s0);
float4x4 viewWorldProjection;
float time;
float4 shaderData;
float3 color;

float4 ShaderPS(float4 vertexColor : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
    return float4(color * 1, .1) * tex2Dbias(spritebatchImage, float4(texCoords.x, texCoords.y, 1, 12)).a;
}

technique t0
{
    pass Outline
    {
        PixelShader = compile ps_3_0 ShaderPS();
    }
}