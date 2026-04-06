#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float4x4 viewWorldProjection;
float time;
float4 shaderData;
float3 color;


float sinBetween(float a, float b, float v)
{
    float h = (b - a) / 2.;
    return a + h + sin
    (v) * h;
}
float PingPong(float value)
{
    value %= 1;
    if (value < 0)
        value += 1;

    if (value >= 0.5)
        return 2 - value * 2;

    return value * 2;
}



// soo cool 

//float2 centerUV = (texCoords * 2 - 1);
//float2 cylinderUV = float2(pow(centerUV.x, 2), centerUV.y);
//float4 tornado = tex2D(image1, centerUV + float2(time - centerUV.y * 5, 0));
//    tornado.rgba *= lerp(1,0,cylinderUV.x);
//    tornado.rgb *= lerp(color + float3(0,0,1), float3(1, 0, 0), tornado.r - centerUV.x);
//    tornado /= tex2D(image1,centerUV + time).r - cylinderUV.x * 5;
//    return
//tornado;
float2 turbulence(float2 p)
{
    float freq = 7.;
    
    float2x2 rot = float2x2(0.6, -0.4, 0.4, 0.6);
    
    for (float i = 0.0; i < 5; i++)
    {
        float phase = freq * mul(p, rot).y + 0.4 * time + i;
        p += -1 * rot[0] * cos(phase) / freq;
        
        rot *= float2x2(0.6, -0.8, 0.8, 0.6);
        freq *= 1.75;
    }
    
    return p;
}
float4 ShaderPS(float4 vertexColor : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
    float2 uv = texCoords;
    
    uv.x += 0.5;
     uv = (uv * 2 - 1);
    float3 ro = float3(0,0,-1);
    float3 rd = normalize(float3(uv.y, uv.x, 1));
    float d = 0;
    float4 col = float4(0,0,0,1);
    float z = 1;
    float cone;
    float3 p;
    for (int i = 0; i < 12; i++)
    {
        p = z * (ro + rd);
        
        p.z -= 5;
        for (d = 0.6; d < 15; d /= 0.1)
            p += cos((p.yzx * 0.125 * (texCoords.x * 3) - float3(-time * 35, 3, -time * 20)) * d) / d;
        p.xy *= 1.35;
        cone = abs(length(p.xz * 2.25) - p.y);
        z += d = cone;
        
    }
    col = lerp(1,0,d);
    col.a = col.r + col.b + col.g;
    col = saturate(col);
    float4 glow = 1 / distance(float2(.5, .5), cone * float2(1.2,1)) / 10;
    glow = smoothstep(0.5, 1, glow);
    col = floor(col * 5) / 5;
    
    return lerp(glow * float4(0, 1, 5, 1), float4(1, 1, 5, 1) * float4(color, 1), col.r);

}

technique t0
{
    pass DukeTornado
    {
        PixelShader = compile ps_3_0 ShaderPS();
    }
}