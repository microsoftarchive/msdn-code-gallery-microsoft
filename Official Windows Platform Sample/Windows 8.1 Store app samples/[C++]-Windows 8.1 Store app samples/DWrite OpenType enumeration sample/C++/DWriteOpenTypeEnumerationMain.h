//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "DWriteOpenTypeEnumerationRenderer.h"

// Renders Direct2D and 3D content on the screen.
namespace DWriteOpenTypeEnumeration
{
    class DWriteOpenTypeEnumerationMain : public DX::IDeviceNotify
    {
    public:
        DWriteOpenTypeEnumerationMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        ~DWriteOpenTypeEnumerationMain();
        void UpdateForWindowSizeChange();
        Platform::Array<bool>^ ReturnSupportedFeatures(int fontNumber);
        void SetStylisticSet(int stylisticSet);
        bool Render();

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample renderer class.
        std::unique_ptr<DWriteOpenTypeEnumerationRenderer> m_sceneRenderer;

        Microsoft::WRL::ComPtr<IDWriteTextAnalyzer2> m_textAnalyzer;
        Microsoft::WRL::ComPtr<IDWriteFontFace2> m_gabriola;
        Microsoft::WRL::ComPtr<IDWriteFontFace2> m_times;
        Microsoft::WRL::ComPtr<IDWriteFontFace2> m_arial;
        Microsoft::WRL::ComPtr<IDWriteFontFace2> m_meiryo;
    };
}