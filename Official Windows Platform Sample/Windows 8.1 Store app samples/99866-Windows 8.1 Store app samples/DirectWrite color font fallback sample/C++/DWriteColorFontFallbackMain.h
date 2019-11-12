//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "DWriteColorFontFallbackRenderer.h"
#include "DWriteColorFontFallback.h"

// Renders Direct2D and 3D content on the screen.
namespace DWriteColorFontFallback
{
    class DWriteColorFontFallbackMain : public DX::IDeviceNotify
    {
    public:
        DWriteColorFontFallbackMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        ~DWriteColorFontFallbackMain();
        void UpdateForWindowSizeChange();
        bool Render();

        // Accessors
        unsigned int GetFontFallbackId() const          { return m_fontFallbackId; };
        bool ColorGlyphsEnabled() const                 { return m_colorGlyphs; };
        float GetZoom() const                           { return m_zoom; };
        D2D1_POINT_2F GetTranslation() const            { return m_viewPosition; };

        void SetFontFallbackId(unsigned int id)         { m_fontFallbackId = id; };
        void UseColorGlyphs(bool enable)                { m_colorGlyphs = enable; };
        void SetZoom(float zoom)                        { m_zoom = zoom; };
        void SetTranslation(D2D1_POINT_2F point)        { m_viewPosition = point; };

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample renderer class.
        std::unique_ptr<DWriteColorFontFallbackRenderer> m_sceneRenderer;

        // State
        unsigned int m_fontFallbackId;
        bool m_colorGlyphs;
        float m_zoom;
        D2D1_POINT_2F m_viewPosition;
    };
}