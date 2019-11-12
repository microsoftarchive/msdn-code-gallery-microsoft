//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "SampleOverlay.h"

#include "DirectXBase.h"
#include "BasicMath.h"
#include "BasicCamera.h"
#include "LonLatController.h"

struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

ref class ResourceLoading : public DirectXBase
{
internal:
    ResourceLoading();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;
    void Update(float timeTotal, float timeDelta);
    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

private:
    SampleOverlay^ m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>        m_inputLayout;
    Microsoft::WRL::ComPtr<ID3D11Buffer>             m_vertexBuffer;
    Microsoft::WRL::ComPtr<ID3D11Buffer>             m_indexBuffer;
    Microsoft::WRL::ComPtr<ID3D11VertexShader>       m_vertexShader;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>        m_pixelShader;
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_textureSRV;
    Microsoft::WRL::ComPtr<ID3D11SamplerState>       m_sampler;
    Microsoft::WRL::ComPtr<ID3D11Buffer>             m_constantBuffer;

    uint32            m_indexCount;
    ConstantBuffer    m_constantBufferData;
    BasicCamera^      m_camera;
    LonLatController^ m_controller;

    bool              m_loadingComplete;
};
