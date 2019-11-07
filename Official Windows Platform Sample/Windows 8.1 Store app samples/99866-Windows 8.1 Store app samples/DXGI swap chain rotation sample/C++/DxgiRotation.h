//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "BasicMath.h"
#include "BasicCamera.h"

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

ref class DxgiRotation : public DirectXBase
{
internal:
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float TimeTotal, float TimeDelta);

private:
    SampleOverlay^  m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;                // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;               // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;                // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;               // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;                // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;                    // cube texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureSRV;                 // cube texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;             // constant buffer resource
    Microsoft::WRL::ComPtr<IWICFormatConverter>         m_referenceBitmapSource;      // oriented reference image source
    Microsoft::WRL::ComPtr<ID2D1Bitmap>                 m_referenceBitmap;            // oriented reference image
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_textBrush;                  // reference text brush
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_textFormat;                 // reference text format
    Microsoft::WRL::ComPtr<IDWriteTextLayout>           m_textLayout;                 // reference text layout

    unsigned int    m_indexCount;                  // cube index count
    ConstantBuffer  m_constantBufferData;          // constant buffer resource data
    BasicCamera^    m_Camera;                      // scene camera
};
