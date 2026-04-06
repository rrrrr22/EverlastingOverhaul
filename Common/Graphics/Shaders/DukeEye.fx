#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float4x4 viewWorldProjection;
float time;
float4 shaderData;
float3 color;

float2 Rotate(float2 uv, float amount)
{
    float2 uv2 = uv;
    float s = sin(amount);
    float c = cos(amount);
    uv2.x = (uv.x * c) + (uv.y * -s);
    uv2.y = (uv.x * s) + (uv.y * c);

    return uv2;
    
}

float2 expandInsideOutside(float2 uv)
{
    float1 t = time + shaderData.w;
    float2 uv2 = Rotate(uv, t);
    float1 d = length(uv2);

    return (d * uv2 + ((t) - uv2));
    
}

float4 Star(float2 uv)
{
    float d = length(uv);
    float angle = atan2(uv.y, uv.x);
    float2 VortexUV = float2(sin(angle + d * .1), d + time);
    
    float glow = 1 / length(uv) / 2;
    float pulse = sin(time * 30) * 0.2 + 1;
    
    
    float starShape = clamp(1 - abs(((uv.x * .9f) * (uv.y * .9f)) * 125), 0, 2);

    glow *= smoothstep(0, 1, glow / 22 * pulse);
    starShape *= smoothstep(0, 1, length(glow) * 100);
    float4 star = smoothstep(0, 1, float4((starShape + glow / 2) * (color) * 2, starShape));
    star = tanh(star);
    return star;
}

float4 ShaderPS(float4 vertexColor : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
    float4 col = float4(0, 0, 0, 0);
    float2 uv = texCoords * 2 - 1;
    

    
    // base
    col += Star(uv);
    
    //glows
    for (float i = 3.141518 * 2; i > 0; i -= 3.141518 * 2 / 16)
    {
        
    }
    
    //float2 uv = texCoords;
    //float4 col = tex2D(image1,uv);
    //col.a *= col.r;
        return col;
}

technique t0
{
    pass Star
    {
        PixelShader = compile ps_2_0 ShaderPS();
    }
}