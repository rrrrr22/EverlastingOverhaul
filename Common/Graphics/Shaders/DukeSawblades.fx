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

float2 strikeShape(float2 uv)
{
    // tail
    float2 tail;
    tail = uv * 2 - 1;
    tail = abs(uv.x * uv.y);
    
    //ball
    float ball = 1 / abs(length(uv * 2 - 1)) / 15;
    
    
    
    return uv;
}

float2 Rotate(float2 uv, float angle, float2 pivot)
{
    float2x2 rotationMatrix = float2x2(cos(angle), sin(angle), -sin(angle),cos(angle));
    uv -= pivot;
    float2 r = mul(rotationMatrix, uv);
    r += pivot;
    return r;
    
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
    float2 uv = texCoords;
    float4 tex = tex2D(image1, Rotate(texCoords, shaderData.x * 1.6, float2(0.5, .5)));
    float4 shineTex = tex2D(image2, Rotate(texCoords, 3.141519 + shaderData.x * 0.8, float2(0.5, .5)));
    shineTex += tex2D(image2, Rotate(texCoords, shaderData.x * 1.6, float2(0.5, .5)));
    tex.a *= tex.r;
    uv = Rotate(uv, shaderData.x, float2(0.5, .5));
    tex.rgb = tex.r * palette((uv.x / 2 + uv.y / 4) - shineTex.r) * float3(1, 0.3, 1) * shineTex.r;
    
    //motion blur

    float4 texBlur = tex2Dlod(image1, float4(uv.x, uv.y,3.5,.2)).r;
    for (float i = 3.141519 * 2; i > 0; i -= 3.141519 / 8)
    {
        uv = Rotate(uv, i, float2(.5,.5));
        texBlur *= tex2D(image1, float2(uv.x, uv.y)).a;

    }
    
        return tex *shaderData.y;
}

technique t0
{
    pass DukeTornado
    {
        PixelShader = compile ps_3_0 ShaderPS();
    }
}