//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

enum class ScreenSavingState
{
    NotSaved,
    Saving,
    Saved
};

ref class SaveAsImageFile : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    SaveAsImageFile();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
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

    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
        );

    void OnDpiChanged(_In_ Windows::Graphics::Display::DisplayInformation^ sender, _In_ Platform::Object^ args);

    void OnDisplayContentsInvalidated(_In_ Windows::Graphics::Display::DisplayInformation^ sender, _In_ Platform::Object^ args);

    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView,
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args
        );

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    // Sample Methods
    void SaveBitmapToFile();
    static void SaveBitmapToStream(
        _In_ Microsoft::WRL::ComPtr<ID2D1Bitmap1> d2dBitmap,
        _In_ Microsoft::WRL::ComPtr<IWICImagingFactory2> wicFactory2,
        _In_ Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext,
        _In_ REFGUID wicFormat,
        _In_ IStream* stream
        );

    SampleOverlay^                                                  m_sampleOverlay;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_blackBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>                       m_textFormat;
    ScreenSavingState                                               m_screenSavingState;
    Platform::String^                                               m_imageFileName;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
