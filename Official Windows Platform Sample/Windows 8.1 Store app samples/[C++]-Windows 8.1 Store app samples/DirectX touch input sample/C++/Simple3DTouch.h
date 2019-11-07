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
#include "MoveLookController.h"
#include "SampleOverlay.h"

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

#define START_POSITION (float3(0, -1.5, 0))
#define ROOM_BOUNDS (float3(4, 3, 6))

ref class Simple3DTouch : public DirectXBase
{
internal:
    Simple3DTouch();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float TimeTotal, float TimeDelta);

    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

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

    unsigned int                     m_indexCount;                  // cube index count
    ConstantBuffer                   m_constantBufferData;          // constant buffer resource data
    BasicCamera^                     m_camera;                      // scene camera
    MoveLookController^              m_controller;                  // controller to map from input events to movement

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_textBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_textFormat;

    Microsoft::WRL::ComPtr<IDWriteTextLayout>           m_moveControlLayout;
    DWRITE_TEXT_METRICS                                 m_moveMetrics;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>           m_lookControlLayout;
    DWRITE_TEXT_METRICS                                 m_lookMetrics;
};
