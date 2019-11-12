//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "BasicTimer.h"
#include "WaveEffect.h"

ref class CustomVertexShaderSample : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    CustomVertexShaderSample();

    void Update();

    // Implement DirectXBase methods.
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

public:
    // Implement IFrameworkView methods.
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    // Implement event handlers.
    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender
        );

    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ sender,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerWheelChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ sender,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    SampleOverlay^                              m_sampleOverlay;

    Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;

    bool                                        m_isWindowClosed;

    Microsoft::WRL::ComPtr<IWICFormatConverter> m_wicFormatConverter;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_bitmapSourceEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_waveEffect;

    D2D1_SIZE_U                                 m_imageSize;
    float                                       m_skewX;
    float                                       m_skewY;

    BasicTimer^                                 m_timer;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
