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
#include "LonLatController.h"
#include "SampleOverlay.h"

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

#define START_POSITION (float3(0, 0.4f, -2))

ref class BumpMapping : public DirectXBase
{
internal:
    BumpMapping();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float TimeTotal, float TimeDelta);

    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

private:
    SampleOverlay^                                      m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;        // mesh vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;       // mesh vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;        // mesh index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;       // vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;        // pixel shader
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;            // color texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureSRV;         // color texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;            // color texture sampler
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_normals;            // normal map texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_normalSRV;          // normal map texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_normalSampler;      // normal map sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;     // constant buffer resource

    unsigned int                     m_indexCount;                  // mesh index count
    ConstantBuffer                   m_constantBufferData;          // constant buffer resource data
    BasicCamera^                     m_camera;                      // scene camera
    LonLatController^                m_controller;                  // controller to map from input events to movement

};
