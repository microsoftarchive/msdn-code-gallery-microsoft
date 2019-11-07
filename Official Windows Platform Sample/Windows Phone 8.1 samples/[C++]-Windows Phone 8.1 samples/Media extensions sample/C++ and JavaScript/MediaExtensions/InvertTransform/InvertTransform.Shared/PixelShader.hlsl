struct VSOutput
{
    float4 Pos : SV_POSITION;              
    float2 Tex : TEXCOORD0;
};

SamplerState samp : register( s0 );
Texture2D<float4> Input : register( t0 );

float4 PSMain( VSOutput Index ) : SV_Target0
{
    float4 color = Input.Sample(samp,Index.Tex);
    color.rgb = 1 - color.rgb;
    return color;
}