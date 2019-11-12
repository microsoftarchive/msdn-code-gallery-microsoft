//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

inline static float Clamp(float v, float low, float high)
{
    return (v < low) ? low : (v > high) ? high : v;
}

ref class D2DTextAnimation : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    D2DTextAnimation();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void Render() override;
    void CreateOpacityMask();

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

    void OnPointerWheelChanged(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    void OnManipulationCompleted(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationCompletedEventArgs^ args
        );

    void OnTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::TappedEventArgs^ args
        );

    void DrawTextLayout(
        float x,
        float y
        );

    void CalculateCenterPosition();

    SampleOverlay^                                          m_sampleOverlay;
    Platform::Agile<Windows::UI::Input::GestureRecognizer>  m_gestureRecognizer;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>            m_blackBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>               m_textFormat;
    Microsoft::WRL::ComPtr<ID2D1Bitmap1>                    m_opacityBitmap;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>               m_textLayout;
    DWRITE_TEXT_METRICS                                     m_textMetrics;
    Windows::Foundation::Point                              m_viewPosition;
    float                                                   m_zoom;
    bool                                                    m_animating;
    bool                                                    m_firstRun;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
