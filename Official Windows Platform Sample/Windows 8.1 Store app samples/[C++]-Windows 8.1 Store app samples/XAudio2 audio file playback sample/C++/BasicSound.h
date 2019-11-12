//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "SampleOverlay.h"
#include "XAudio2SoundPlayer.h"
#include "DirectXBase.h"

ref class BasicSound : public DirectXBase
{
internal:
    BasicSound();

    // Overrides from DirectXBase
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void OnSuspending();
    void OnResuming();

    // Additional event handler not defined in DirectXBase
    void OnPointerPressed(_In_ Windows::UI::Input::PointerPoint^ p);

private:
    ~BasicSound();

    // Overlay for generic sample graphics
    SampleOverlay^              m_sampleOverlay;

    // Object to maintain and play a list of sounds via XAudio2
    XAudio2SoundPlayer*         m_soundPlayer;

    // Display related items
    float                       m_rectangleWidth;
    float                       m_rectangleHeight;

    // Rectangle painting brushes
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_whiteBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_greenBrush;

    // Members for drawing text messages
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_dataTextFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textBrush;
};
