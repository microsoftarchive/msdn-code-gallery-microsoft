//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"
#include "SampleOverlay.h"
#include "BasicReaderWriter.h"

namespace DirectXMapDefaultBuffers
{
    // This sample renderer instantiates a basic rendering pipeline.
    class DirectXMapDefaultBuffersRenderer
    {
    public:
        DirectXMapDefaultBuffersRenderer(
            const std::shared_ptr<DX::DeviceResources>& deviceResources,
            const std::shared_ptr<SampleOverlay>& sampleOverlay
            );

        void CreateDeviceDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();

    private:

        void InitializeProjectileBuffer();
        void InitializeCollisionBuffer();
        void InitializeStagingBuffers();

        void RenderCollisionCount();
        void RenderObjects();
        void RenderWalls();

        // Cached pointers to device resources and sample overlay.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        std::shared_ptr<SampleOverlay> m_sampleOverlay;

        // Sample-specific resources.
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_projectileBuffer;
        Microsoft::WRL::ComPtr<ID3D11UnorderedAccessView> m_projectileBufferView;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_collisionBuffer;
        Microsoft::WRL::ComPtr<ID3D11UnorderedAccessView> m_collisionBufferView;
        Microsoft::WRL::ComPtr<ID3D11ComputeShader> m_physicsShader;

        // Staging buffers will be used to access GPU resources on the CPU if MapDefault
        // functionality is not available.
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_projectileStagingBuffer;
        Microsoft::WRL::ComPtr<ID3D11UnorderedAccessView> m_projectileStagingBufferView;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_collisionStagingBuffer;
        Microsoft::WRL::ComPtr<ID3D11UnorderedAccessView> m_collisionStagingBufferView;

        // Resources to render buffer contents to the screen.
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_whiteBrush;
        Microsoft::WRL::ComPtr<IDWriteTextFormat> m_collisionCountTextFormat;

        enum CpuAccessMethod
        {
            MapDefaultBuffers,
            MapStagingBuffers
        } m_cpuAccessMethod;

        struct Projectile
        {
            float positionX;
            float positionY;
            float velocityX;
            float velocityY;
        };
    };
}
