//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXHelper.h"
#include "DeviceResources.h"

namespace TiledResources
{
    class ControlsOverlay
    {
    public:
        ControlsOverlay(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void ReleaseDeviceDependentResources();
        void Render();
        void EvaluateTouchControl(float touchX, float touchY, float& x, float& y, float& z, float& rx, float& ry, float& rz);

    private:
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        Microsoft::WRL::ComPtr<ID2D1Bitmap> m_controlsBitmap;

        const float m_paddingX;
        const float m_paddingY;
        const float m_overlaySize;
    };
}
