//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "StepTimer.h"
#include "DeviceResources.h"
#include "ShadowSceneRenderer.h"

// Renders Direct2D and 3D content on the screen.
namespace ShadowMapping
{
    class ShadowMapSampleMain : public DX::IDeviceNotify
    {
    public:
        ShadowMapSampleMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        ~ShadowMapSampleMain();
        void UpdateForWindowSizeChange();
        void Update();
        bool Render();

        // Property pass-through methods.
        void  SetFiltering(bool useLinear);
        bool  GetFiltering();
        void  SetShadowSize(float size);
        float GetShadowSize();
        bool  GetD3D9ShadowsSupported();

        // Methods for saving/loading the internal state of the app.
        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample renderer class.
        std::unique_ptr<ShadowSceneRenderer> m_shadowSceneRenderer;

        // Rendering loop timer.
        DX::StepTimer m_timer;
    };
}