//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"
#include "BasicShapes.h"
#include "BasicLoader.h"
#include "BasicCamera.h"
#include "LonLatController.h"
#include <DirectXColors.h>
#define NumberOfFaces 6
#define StartPosition (float3(0, 0.4f, -5))

struct ModelViewProjectionConstantBuffer
{
    DirectX::XMFLOAT4X4 model;
    DirectX::XMFLOAT4X4 view;
    DirectX::XMFLOAT4X4 projection;
};

namespace DynamicCubemap
{
    // This sample renderer instantiates a basic rendering pipeline.
    class DynamicCubemapRenderer
    {
    public:
        DynamicCubemapRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();

        void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

    private:
        void RenderCubeModel(DirectX::XMFLOAT3 positionOffset, DirectX::XMFLOAT4X4 rotation);
        void RenderSphereModel(DirectX::XMFLOAT3 positionOffset, DirectX::XMFLOAT4X4 rotation);
        void RenderScene();
        
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample-specific resources.
        // Members for the texture that appears on the cube.
        Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;                    // cube texture
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureShaderResourceView;  // cube texture view
        Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler

        // Members for the cube object.
        Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayoutCube;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBufferCube;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBufferCube;
        Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShaderCube;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShaderCube;
        uint32                                              m_indexCountCube;

        // Members for position/orientation of the cube objects.
        DirectX::XMFLOAT3                                   m_cube1Offset;
        DirectX::XMFLOAT3                                   m_cube2Offset;
        DirectX::XMFLOAT4X4                                 m_cubeRotation;
        DirectX::XMFLOAT4X4                                 m_cube2Rotation;


        // Members for the sphere object.
        Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayoutSphere;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBufferSphere;
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBufferSphere;
        Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShaderSphere;
        Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShaderSphere;
        uint32                                              m_indexCountSphere;

        // Members for position/orientation of the sphere object.
        DirectX::XMFLOAT3                                   m_sphereOffset;
        DirectX::XMFLOAT4X4                                 m_sphereRotation;

        // Members for the Constant buffer.
        Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;
        ModelViewProjectionConstantBuffer                   m_constantBufferData;

        // Members for the various cameras used.
        BasicCamera^                    m_camera;                   // The camera used to present the entire scene.
        BasicCamera^                    m_environmentMapCamera;     // The camera used to present the environment map views.
        LonLatController^               m_controller;               // The controller to map from input events to movement.

        // Members for the environment map. 
        Microsoft::WRL::ComPtr<ID3D11RenderTargetView>      m_environmentMapRenderTargetView[NumberOfFaces];
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_environmentMapShaderResourceView;
        Microsoft::WRL::ComPtr<ID3D11DepthStencilView>      m_environmentMapDepthStencilView;
        Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_environmentMapDepthStencilTexture;
        Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_environmentMapTexture;
        uint m_environmentMapWidth;         // The width of the texture on each cubemap face.
        uint m_environmentMapHeight;        // The height of the texture on each cubemap face.
    };
}
