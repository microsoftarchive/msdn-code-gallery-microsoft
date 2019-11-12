//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "BasicMath.h"
#include "SampleOverlay.h"
#include "DirectXMath.h"
#include "Stereo3DMatrixHelper.h"

// The constant buffer that is used with the DirectXMath library to draw the cube.
struct ConstantBuffer
{
    DirectX::XMFLOAT4X4 model;
    DirectX::XMFLOAT4X4 view;
    DirectX::XMFLOAT4X4 projection;
};

ref class StereoSimpleD3D : public DirectXBase
{
internal:
    StereoSimpleD3D();
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    float GetStereoExaggeration();
    void RenderEye(_In_ unsigned int eyeIndex);
    void SetStereoExaggeration(_In_ float currentExaggeration);
    void Update(
        _In_ unsigned int eyeIndex,
        _In_ float timeTotal,
        _In_ float timeDelta
        );

private:
    SampleOverlay^                                      m_sampleOverlay;
    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;                // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;               // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;                // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;               // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;                // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureShaderResourceView;  // cube texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;             // constant buffer resource
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_brush;                      // brush for message drawing
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_textFormat;                 // text format for message drawing

    unsigned int             m_indexCount;                  // cube index count
    ConstantBuffer           m_constantBufferData;          // constant buffer resource data
    float                    m_projAspect;                  // aspect ratio for projection matrix
    float                    m_nearZ;                       // nearest Z-distance at which to draw vertices
    float                    m_farZ;                        // farthest Z-distance at which to draw vertices
    float                    m_widthInInches;               // estimated screen width in inches
    float                    m_heightInInches;              // estimated screen height in inches
    float                    m_stereoExaggerationFactor;    // stereo effect that is user adjustable
    Platform::String^        m_hintMessage;                 // hint message about customer manipulation
    float                    m_worldScale;                  // developer specified world unit
};
