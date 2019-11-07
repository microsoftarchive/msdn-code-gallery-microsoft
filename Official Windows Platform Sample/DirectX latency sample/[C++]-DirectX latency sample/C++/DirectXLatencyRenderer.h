//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"

namespace DirectXLatency
{
    // This sample renderer instantiates a basic rendering pipeline.
    class DirectXLatencyRenderer
    {
    public:
        DirectXLatencyRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();
        void SetCirclePosition(Windows::Foundation::Point newPosition);
        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // A brush with which to draw the circle.
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_brush;

        // The current position of the circle.
        Windows::Foundation::Point m_currentPosition;

        // The circle to be drawn.
        D2D1_ELLIPSE m_ellipse;
    };
}
