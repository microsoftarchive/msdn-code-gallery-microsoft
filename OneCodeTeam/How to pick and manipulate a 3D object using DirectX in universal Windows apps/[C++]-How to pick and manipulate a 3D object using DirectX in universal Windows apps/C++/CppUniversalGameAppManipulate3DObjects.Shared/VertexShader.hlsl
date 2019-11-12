cbuffer cbMVPData
{
	row_major matrix model;
	row_major matrix view;
	row_major matrix projection;
};
struct VertexInput
{
	float3 pos : POSITION;
	float3 color : COLOR;
};
struct VertexOutput
{
	float4 pos : SV_POSITION;
	float3 color : COLOR;
};
VertexOutput main( VertexInput input)
{
	VertexOutput output;
	float4 pos = float4( input.pos, 1.0f);

	pos = mul(pos, model);
	pos = mul(pos, view);
	pos = mul(pos, projection);
	output.pos = pos;
	output.color = input.color;
	return output;
}