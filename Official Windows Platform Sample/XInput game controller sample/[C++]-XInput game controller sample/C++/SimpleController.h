//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

ref class SimpleController : public DirectXBase
{
internal:
    SimpleController();

    // Overrides from DirectXBase
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float timeTotal, float timeDelta);

private:
    // Input functionality
    void FetchControllerInput();
    void RenderControllerInput();

    // Functions for drawing information on screen
    void DrawHeader(const wchar_t* pText, const D2D1_RECT_F& loc);
    void DrawText(const wchar_t* pText, const D2D1_RECT_F& loc);
    void DrawText(uint32 value, const D2D1_RECT_F& loc);
    void DrawText(int16 value, const D2D1_RECT_F& loc);
    void DrawText(uint8 value, const D2D1_RECT_F& loc);
    void DrawButtonText(uint16 buttons, const D2D1_RECT_F& loc);

    // Overlay for default sample graphics
    SampleOverlay^          m_sampleOverlay;

    // Input related members
    bool                    m_isControllerConnected;  // Do we have a controller connected
    XINPUT_CAPABILITIES     m_xinputCaps;             // Capabilites of the controller
    XINPUT_STATE            m_xinputState;            // The current state of the controller
    uint64                  m_lastEnumTime;           // Last time a new controller connection was checked

    // Members for drawing
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_headerTextFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_dataTextFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textBrush;
};
