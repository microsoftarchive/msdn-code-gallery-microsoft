//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "MultiViewSampleRenderer.h"
#include "SampleOverlay.h"
#include <DirectXColors.h>
#include "DirectXHelper.h"

// Renders Direct2D and 3D content on the screen.
namespace MultiViewSample
{
    class MultiViewSampleMain : public DX::IDeviceNotify
    {
    public:
        MultiViewSampleMain(const std::shared_ptr<DX::DeviceResources>& deviceResources, const std::wstring& viewTitle, Platform::String^ textToDraw, const float windowColorRGBA[4]);
        ~MultiViewSampleMain();
        void UpdateForWindowSizeChange();
        bool Render();

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        float m_windowColorRGBA[4];

        // Sample renderer class.
        std::unique_ptr<MultiViewSampleRenderer> m_sceneRenderer;

        // Sample overlay class.
        std::unique_ptr<SampleOverlay> m_sampleOverlay;
    };
}