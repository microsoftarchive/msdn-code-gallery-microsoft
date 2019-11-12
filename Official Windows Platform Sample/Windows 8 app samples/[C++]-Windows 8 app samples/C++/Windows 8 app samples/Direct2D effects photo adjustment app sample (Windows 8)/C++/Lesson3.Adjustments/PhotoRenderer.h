//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <roapi.h>

#include "DirectXBase.h"
#include "SampleOverlay.h"

ref class PhotoRenderer : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    PhotoRenderer();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

public:
    // IFrameworkView Methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    // Event Handlers
    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender
        );

    void OnDisplayContentsInvalidated(
        _In_ Platform::Object^ sender
        );

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnResuming(
        _In_ Platform::Object^ sender,
        _In_ Platform::Object^ args
        );

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    // GestureRecognizer event handlers
    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    void OnTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::TappedEventArgs^ args
        );

    inline static float Clamp(float v, float low, float high)
    {
        return (v < low) ? low : (v > high) ? high : v;
    }

    void ResetAdjustmentState();
    void UpdateAdjustmentEffectValues();

    SampleOverlay^                              m_sampleOverlay;

    D2D1_SIZE_U                                 m_bitmapPixelSize;
    D2D1_SIZE_F                                 m_contextSize;

    float                                       m_saturation;
    float                                       m_brightness;

    Microsoft::WRL::ComPtr<IWICFormatConverter> m_wicFormatConverter;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_bitmapSourceEffect;

    // Lesson 3:
    // Declare the photo adjustment effects
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_brightnessEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_saturationEffect;

    Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
