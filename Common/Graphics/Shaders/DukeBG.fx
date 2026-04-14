#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float4x4 viewWorldProjection;
float time;
float4 shaderData;
float3 color;
float2 CameraPositionMovement;


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
    
    float2x2 rot = float2x2(0.6, -0.8, 0.8, 0.6);
    
    for (float i = 0.0; i < 15; i++)
    {
        float phase = freq * mul(p,rot).y + 0.4 * time + i;
        p += -1 * rot[0] * cos(phase) / freq;
        
        rot *= float2x2(0.6, -0.8, 0.8, 0.6);
        freq *= 1.75;
    }
    
    return p;
}

float3 OneBasedRGB(float3 rgb)
{
    return rgb / 255;
}
float3 palette(float t)
{
    float3 a = float3(-0.632
    ,0.448
    ,0.448);
    float3 b = float3(0.000
    ,0.500
    ,0.500);
    float3 c = float3(0.000
    ,0.500
    ,0.448);
    float3 d = float3(2.088
    ,0.468
    ,0.667);

    return a + b * cos(6.28318 * (c * t + d));
}
float2 Rotate(float2 uv, float angle, float2 pivot)
{
    float2x2 rotationMatrix = float2x2(cos(angle), sin(angle), -sin(angle), cos(angle));
    uv -= pivot;
    float2 r = mul(rotationMatrix, uv);
    r += pivot;
    return r;
    
}

float4 ShaderPS(float4 vertexColor : COLOR0, float2 texCoords : TEXCOORD0) : COLOR0
{
    //float2 uv = texCoords * float2(1,1 / exp(texCoords.y) * 0.5);
    //uv.x = sin(texCoords.x * 2 - 1);
    //float4 tex1 = tex2D(image1, uv + shaderData.xy / 5120 + time * 0.05) * float4(0.5, 0.25, 1, 1);
    //float4 texClouds = tex2D(image1, uv * float2(1, 3) + shaderData.xy / 3840 + time * 0.01) * float4(uv.y, uv.y, uv.y, 1) * float4(0, 0.2, 2, 1);
    //return tex1 * tex2D(image1, uv + shaderData.xy / 1920 + time * 0.03) * float4(1, 0.25, 1, 1) + texClouds;
    
    //texCoords *= 2 - 1;
    //texCoords -= float2(.5, .5);
    //texCoords *= shaderData.x;
    //texCoords += float2(.5, .5);
    //float2 ogCoords = texCoords; //float2(texCoords.x, (sin(texCoords.x + time) + time) * 0.1 - texCoords.y);
    //texCoords.y = exp2(texCoords.y);
    //texCoords = Rotate(texCoords, 3.141519, float2(.5,.5));
    //float thunder = 1 / frac((time) * 0.25) * 0.3;
    //float4 col = float4(0, 0, 0,0);
    //texCoords *= float2(1,.7);
    //for (int i = 0; i < 3; i++)
    //{
    //    float2 uv = (texCoords - CameraPositionMovement * 0.25 ) * 2 - 1 ;
    //    uv += uv * i * 0.1;
    //    float2 wobbleUV = float2(uv.x + i * 0.7, exp(sin(uv.y * .75 + time * 0.5)));
    
    //    float4 texTEMP = tex2D(image1, wobbleUV + float2(.5 + exp2(i * 1.41), time * .4 + .25));
    //    float4 texTEMP2 = tex2D(image1, wobbleUV + float2(time * 0.1, 0));
    //    float4 texTEMP3 = tex2D(image1, wobbleUV + float2(-time * 0.1 + .8, .1));

    //    float4 tex = tex2D(image1, uv + float2(.2 + exp2(i * 1.41), time * 0.4)) + texTEMP / 2;
    //    tex.r *= texTEMP2.r;
    //    tex.r *= texTEMP3.r;
    //    tex.rgb *= palette(tex.r + cos(time + tex.r ) * 0.1) / 7;
    //    tex.rgb *= clamp(thunder * 3, 1, thunder * 3);
        
    //    col += tex;
        
    //}

    //float4 edge = float4((ogCoords.y) * 3, (ogCoords.y) * 3, (ogCoords.y) * 3, (ogCoords.y) * 3);
    //col.rgba *= smoothstep(col.rgba, float4(0,0,0,0), edge);
    //return col;

    
    float4 col = 0;
    texCoords.x -= .5;
    texCoords.x *= shaderData.x;
    texCoords.x += .5;
    float2 grad = length((texCoords.y * 2));
    for (int i = 0; i < 5; i++)
    {
        float UVy = (texCoords.y - 0.25);
        float UVx = (texCoords.x - time * 0.01);
        float2 uv = float2(sin(time + texCoords.y * 3) * 0.075 + texCoords.x, UVy - 0.5);
        float4 texMask = tex2D(image1, float2(UVx, uv.y));
        float4 texMask2 = tex2D(image1, float2(-UVx, uv.y));
        float4 tex = tex2D(image1, float2(uv.x - i * 0.1, uv.y - i * 0.01 * 0.67 - 0.5));
        tex.rgb *= palette(sin(tex.r + i * 0.02)+ texMask.r + texMask2.r);
        col += tex / 7;
    }
    
    return col * grad.y;
    
}

technique t0
{
    pass DukeBG
    {
        PixelShader = compile ps_3_0 ShaderPS();
    }
}