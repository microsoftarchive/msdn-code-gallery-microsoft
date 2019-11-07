//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"

namespace Scenario2Component
{
    struct ModelViewProjectionConstantBuffer
    {
        DirectX::XMMATRIX model;
        DirectX::XMMATRIX view;
        DirectX::XMMATRIX projection;
    };

    struct VertexPositionColor
    {
        DirectX::XMFLOAT3 pos;
        DirectX::XMFLOAT3 color;
    };

    public ref class Scenario2ImageSource sealed : Windows::UI::Xaml::Media::Imaging::SurfaceImageSource
    {
    public:
        Scenario2ImageSource(int pixelWidth, int pixelHeight, bool isOpaque);

        void BeginDraw();
        void EndDraw();

        void Clear(Windows::UI::Color color);
        void RenderNextAnimationFrame();

    private protected:
        void CreateDeviceIndependentResources();
        void CreateDeviceResources();

        Microsoft::WRL::ComPtr<ISurfaceImageSourceNative>   m_sisNative;

        // Direct3D objects
        Microsoft::WRL::ComPtr<ID3D11Device>                m_d3dDevice;
        Microsoft::WRL::ComPtr<ID3D11DeviceContext>         m_d3dContext;
        Microsoft::WRL::ComPtr<ID3D11RenderTargetView>      m_renderTargetView;
        Microsoft::WRL::ComPtr<ID3D11DepthStencilView>      m_depthStencilView;
        Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;
        Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;

        ModelViewProjectionConstantBuffer                   m_constantBufferData;

        uint32                                              m_indexCount;

        int                                                 m_width;
        int                                                 m_height;

        float                                               m_frameCount;
    };
}
