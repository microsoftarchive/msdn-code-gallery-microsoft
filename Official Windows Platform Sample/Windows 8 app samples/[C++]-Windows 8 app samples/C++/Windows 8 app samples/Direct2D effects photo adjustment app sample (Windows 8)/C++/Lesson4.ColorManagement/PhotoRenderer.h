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

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
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

    // Lesson 4:
    // Handle the event when the display color profile has changed.
    void OnColorProfileChanged(_In_ Platform::Object^ sender);

    void UpdateDisplayColorContext(
        _In_ Windows::Storage::Streams::DataReader^ colorProfileDataReader
        );

    SampleOverlay^                              m_sampleOverlay;

    D2D1_SIZE_U                                 m_bitmapPixelSize;

    Microsoft::WRL::ComPtr<IWICColorContext>    m_wicColorContext;
    Microsoft::WRL::ComPtr<IWICFormatConverter> m_wicFormatConverter;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_bitmapSourceEffect;

    // Lesson 4:
    // Declare two ColorManagement effects. The first converts from the image's
    // embedded color space to a working space (sRGB). The second converts to
    // the display's color space.
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_colorManagementEffectWorkingSpace;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_colorManagementEffectDisplaySpace;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
