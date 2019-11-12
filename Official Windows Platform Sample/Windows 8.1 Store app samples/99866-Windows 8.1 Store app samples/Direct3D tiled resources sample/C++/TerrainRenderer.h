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
#include "ResidencyManager.h"

namespace TiledResources
{
    // Renders the terrain.
    class TerrainRenderer
    {
    public:
        TerrainRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        concurrency::task<void> CreateDeviceDependentResourcesAsync();
        void ReleaseDeviceDependentResources();

        ID3D11Texture2D* GetDiffuseTexture();
        void SetDiffuseResidencyMap(ID3D11ShaderResourceView* view);
        ID3D11Texture2D* GetNormalTexture();
        void SetNormalResidencyMap(ID3D11ShaderResourceView* view);

        // Update and set state for Input Assembler through Vertex Shader stages.
        // This state will be used by the SamplingRenderer class.
        void SetSourceGeometry(FreeCamera const& camera);

        // Update and set state for Pixel Shader through Output Merger stages.
        void SetTargetsForRendering(FreeCamera const& camera, DX::StepTimer const& timer);

        // Draw the terrain geometry. This is called for both the sampling pass and final render.
        void Draw();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Terrain rendering resources.
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_indexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11InputLayout> m_inputLayout;
        Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexShaderConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_pixelShaderConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11SamplerState> m_anisotropicSampler;
        Microsoft::WRL::ComPtr<ID3D11SamplerState> m_maxFilterSampler;
        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_diffuseTexture;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_diffuseTextureView;
        ID3D11ShaderResourceView* m_diffuseTextureResidencyView;
        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_normalTexture;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_normalTextureView;
        ID3D11ShaderResourceView* m_normalTextureResidencyView;

        unsigned int m_terrainIndexCount;
    };
}
