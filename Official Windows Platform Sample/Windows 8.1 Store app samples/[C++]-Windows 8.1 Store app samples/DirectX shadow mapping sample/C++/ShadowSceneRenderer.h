//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "ShaderStructures.h"
#include "StepTimer.h"

namespace ShadowMapping
{
    // This sample renderer instantiates a basic rendering pipeline.
    class ShadowSceneRenderer
    {
    public:
        ShadowSceneRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();

        __forceinline bool  GetFiltering()                      { return m_useLinear;                 };
        __forceinline void  SetFiltering(bool useLinear)        { m_useLinear = useLinear;            };
        __forceinline float GetShadowDimension()                { return m_shadowMapDimension;        };
        __forceinline void  SetShadowDimension(float dimension) { m_shadowMapDimension = dimension;   };
        __forceinline bool  GetD3D9ShadowsSupported()           { return m_deviceSupportsD3D9Shadows; };

    private:
        void DetermineShadowFeatureSupport();
        void InitShadowMap();
        void RenderShadowMap();
        void RenderSceneWithShadows();
        void RenderQuad();
        void UpdateAllConstantBuffers();

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Direct3D resources for cube geometry.
        Microsoft::WRL::ComPtr<ID3D11InputLayout>   m_inputLayout;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_vertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_indexBuffer;
        Microsoft::WRL::ComPtr<ID3D11VertexShader>  m_vertexShader;
        Microsoft::WRL::ComPtr<ID3D11VertexShader>  m_simpleVertexShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>   m_shadowPixelShader_point;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>   m_shadowPixelShader_linear;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>   m_comparisonShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>   m_textureShader;

        // Shadow buffer Direct3D resources.
        Microsoft::WRL::ComPtr<ID3D11Texture2D>          m_shadowMap;
        Microsoft::WRL::ComPtr<ID3D11DepthStencilView>   m_shadowDepthView;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_shadowResourceView;
        Microsoft::WRL::ComPtr<ID3D11SamplerState>       m_comparisonSampler_point;
        Microsoft::WRL::ComPtr<ID3D11SamplerState>       m_comparisonSampler_linear;
        Microsoft::WRL::ComPtr<ID3D11SamplerState>       m_linearSampler;

        // Model, view, projection constant buffers.
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_cubeViewProjectionBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_lightViewProjectionBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_orthoViewProjectionBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_staticModelBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_rotatedModelBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_orthoTransformBuffer;

        // Render states for front face/back face culling.
        Microsoft::WRL::ComPtr<ID3D11RasterizerState> m_shadowRenderState;
        Microsoft::WRL::ComPtr<ID3D11RasterizerState> m_drawingRenderState;

        // Direct3D resources for displaying the shadow map.
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_vertexBufferQuad;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_indexBufferQuad;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_vertexBufferFloor;
        Microsoft::WRL::ComPtr<ID3D11Buffer>        m_indexBufferFloor;
        D3D11_VIEWPORT                              m_shadowViewport;

        // System resources.
        ViewProjectionConstantBuffer m_cubeViewProjectionBufferData;
        ViewProjectionConstantBuffer m_lightViewProjectionBufferData;
        ViewProjectionConstantBuffer m_orthoViewProjectionBufferData;
        ModelConstantBuffer m_staticModelBufferData;
        ModelConstantBuffer m_rotatedModelBufferData;
        ModelConstantBuffer m_orthoTransformBufferData;
        uint32  m_indexCountQuad;
        uint32  m_indexCountFloor;
        uint32  m_indexCountCube;

        // Variables used with the rendering loop.
        bool    m_loadingComplete;
        float   m_degreesPerSecond;
        bool    m_useLinear;

        // Controls the size of the shadow map.
        float   m_shadowMapDimension;

        // Cached copy of SupportsDepthAsTextureWithLessEqualComparisonFilter.
        bool    m_deviceSupportsD3D9Shadows;
    };
}

