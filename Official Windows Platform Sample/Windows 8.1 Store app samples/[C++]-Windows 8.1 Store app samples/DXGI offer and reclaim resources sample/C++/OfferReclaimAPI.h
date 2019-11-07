//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "BasicMath.h"
#include "BasicCamera.h"
#include "SampleOverlay.h"

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

ref class OfferReclaimAPI : public DirectXBase
{
internal:
    OfferReclaimAPI();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float TimeTotal, float TimeDelta);

    void OnSuspending();
    void OnResuming();
private:
    SampleOverlay^                                      m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;                // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;               // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;                // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;               // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;                // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;                    // cube texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureSRV;                 // cube texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;             // constant buffer resource

    unsigned int             m_indexCount;                  // cube index count
    ConstantBuffer           m_constantBufferData;          // constant buffer resource data
    BasicCamera^             m_Camera;                      // scene camera
};
