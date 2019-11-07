//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"
#include "FreeCamera.h"

namespace TiledResources
{
    // A decoded sample from a sampling render pass.
    struct DecodedSample
    {
        float u;
        float v;
        short mip;
        short face;
    };

    // Renders a low-resolution version of the world, encoding sampled data instead of colors.
    class SamplingRenderer
    {
    public:
        SamplingRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        concurrency::task<void> CreateDeviceDependentResourcesAsync();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void SetTargetsForSampling();
        void RenderVisualization();
        std::vector<DecodedSample> CollectSamples();

        void SetDebugMode(bool value);
        void DebugSample(float x, float y);

    private:
        bool GetNextSamplePosition(int* row, int* column);
        DecodedSample DecodeSample(unsigned int encodedSample);

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_colorTexture;
        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_colorStagingTexture;
        Microsoft::WRL::ComPtr<ID3D11RenderTargetView> m_colorTextureRenderTargetView;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_colorTextureView;
        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_depthTexture;
        Microsoft::WRL::ComPtr<ID3D11DepthStencilView> m_depthTextureDepthStencilView;
        Microsoft::WRL::ComPtr<ID3D11PixelShader> m_samplingPixelShader;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_pixelShaderConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_viewerVertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_viewerIndexBuffer;
        Microsoft::WRL::ComPtr<ID3D11InputLayout> m_viewerInputLayout;
        Microsoft::WRL::ComPtr<ID3D11VertexShader> m_viewerVertexShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader> m_viewerPixelShader;
        Microsoft::WRL::ComPtr<ID3D11SamplerState> m_viewerSampler;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_viewerVertexShaderConstantBuffer;
        CD3D11_VIEWPORT m_viewport;
        unsigned int m_sampleIndex;
        unsigned int m_sampleCount;

        bool m_debugMode;
        bool m_newDebugSamplePosition;
        float m_debugSamplePositionX;
        float m_debugSamplePositionY;
    };
}
