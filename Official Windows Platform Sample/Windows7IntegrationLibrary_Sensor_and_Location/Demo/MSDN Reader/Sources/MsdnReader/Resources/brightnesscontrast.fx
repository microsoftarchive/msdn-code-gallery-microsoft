sampler2D input : register(s0);
float brightness : register(c0);
float contrast : register(c1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv); 
    float4 result = color;
    result = color + brightness;
    result = result * (1.0+contrast)/1.0;
    
    return result;
}