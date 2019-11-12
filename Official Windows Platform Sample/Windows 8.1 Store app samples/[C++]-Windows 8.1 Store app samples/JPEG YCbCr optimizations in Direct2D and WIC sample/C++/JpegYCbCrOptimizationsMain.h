//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "JpegYCbCrOptimizationsRenderer.h"
#include "JpegYCbCrOptimizations.h"

// Renders Direct2D and 3D content on the screen.
namespace JpegYCbCrOptimizations
{
    class JpegYCbCrOptimizationsMain : public DX::IDeviceNotify
    {
    public:
        JpegYCbCrOptimizationsMain(
            const std::shared_ptr<DX::DeviceResources>& deviceResources,
            _In_ ResourcesLoadedHandler^ handler,
            bool isBgraForced
            );
        ~JpegYCbCrOptimizationsMain();

        void UpdateForWindowSizeChange();
        bool Render();

        bool GetIsBgraForced() { return m_isForcedBgraMode; }
        void SetIsBgraForced(bool isBgraForced);

        // IDeviceNotify
        virtual concurrency::task<void> OnDeviceLostAsync();
        virtual void OnDeviceRestored();

    private:
        void CreateRenderer();

        // Handler is provided by DirectXPage and cached here.
        ResourcesLoadedHandler^ m_resourcesLoadedHandler;

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample renderer class.
        std::unique_ptr<JpegYCbCrOptimizationsRenderer> m_sceneRenderer;

        // Indicates whether the renderer should use BGRA resources regardless
        // of whether the YCbCr configuration is supported.
        bool m_isForcedBgraMode;

        // Whether JpegYCbCrOptimizationsRenderer::InvalidateAsync is executing.
        bool m_isInvalidatingRenderer;
    };
}