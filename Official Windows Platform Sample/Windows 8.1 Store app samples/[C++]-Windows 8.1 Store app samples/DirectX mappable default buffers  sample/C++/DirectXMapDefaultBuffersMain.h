//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "StepTimer.h"
#include "DeviceResources.h"
#include "DirectXMapDefaultBuffersRenderer.h"
#include "SampleOverlay.h"

// Renders Direct2D and 3D content on the screen.
namespace DirectXMapDefaultBuffers
{
    class DirectXMapDefaultBuffersMain : public DX::IDeviceNotify
    {
    public:
        DirectXMapDefaultBuffersMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        ~DirectXMapDefaultBuffersMain();
        void UpdateForWindowSizeChange();
        void Update();
        bool Render();

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample renderer class.
        std::unique_ptr<DirectXMapDefaultBuffersRenderer> m_sceneRenderer;

        // Sample overlay class.
        std::shared_ptr<SampleOverlay> m_sampleOverlay;

        // Rendering loop timer.
        DX::StepTimer m_timer;
    };
}