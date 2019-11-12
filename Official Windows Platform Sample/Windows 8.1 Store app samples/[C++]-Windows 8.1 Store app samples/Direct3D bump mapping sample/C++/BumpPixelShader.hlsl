// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//----------------------------------------------------------------------

Texture2D SimpleTexture : register( t0 );
SamplerState SimpleSampler : register( s0 );

Texture2D NormalTexture : register( t1 );
SamplerState NormalSampler : register( s1 );

struct sPSInput
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float3 utan : TANGENT0;		// unit tangent vector in texture u-direction
    float3 vtan : TANGENT1;		// unit tangent vector in texture v-direction
};

float4 SimplePixelShader( sPSInput input ) : SV_TARGET
{
    float3 vToLightW = normalize( float3(1.0,0.2,-0.4) );   // vector towards light source (Worldcoords)  (should be in CB fed by UI)

    // load color texture
    float3 color  = SimpleTexture.Sample( SimpleSampler, input.tex ).rgb;

    // load normal map texture
    float4 nmap = NormalTexture.Sample( NormalSampler, input.tex );
    float3 normal = nmap.rgb*2 - 1;                         // convert to range [-1 .. 1]
    float height = nmap.a;                                  // height is in alpha channel

	// A cross product is about as fast as an interpolator/iterator
    float3 input_norm = cross( input.vtan, input.utan );	// get normal from cross of tangents
    input_norm = normalize( input_norm );

	// Assemble the tangent basis matrix
    float3x3 mTangent = float3x3( normalize( input.utan ),
                                  normalize( input.vtan ),
                                  input_norm );

#if TANGENTSPACE_LIGHTING
    float3 vToLight = mul( mTangent, vToLightW );			// transform light vector into tangent coords
    float intensity = saturate( dot( normal, vToLight) );
#else  // Worldspace lighting
    float3 normalW = mul( normal, mTangent );				// transform sampled normal from tangent space into world space
    float intensity = saturate( dot( normalW, vToLightW) );
#endif

    // Apply a shadow based on height above a sphere
    float costh = dot( input_norm, vToLightW);              // vertex/sphere normal
    float sin = sqrt(1 - costh*costh);                      // compute sin
    if ( (costh < 0) && (sin + height*0.02 < 1)  )          // assume max bump height is 2% of radius
        intensity = 0;

    intensity = 0.99f*intensity + 0.01f;                    // add a small ambient light contribution
    intensity = sqrt( intensity );                          // gamma correct the lighting term

    color *= intensity;                                     // apply lighting to color texture

	return float4(color,1.0);
}
