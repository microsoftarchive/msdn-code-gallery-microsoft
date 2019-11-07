//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

ref class CompositeModes : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    CompositeModes();

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
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    ~CompositeModes();

    inline static float Clamp(float v, float low, float high)
    {
        return (v < low) ? low : (v > high) ? high : v;
    }

    void UpdateAlpha(_In_ float verticalPositionDelta);

    SampleOverlay^                                      m_sampleOverlay;

    Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;

    Microsoft::WRL::ComPtr<IWICFormatConverter>         m_appleBitmap;
    Microsoft::WRL::ComPtr<IWICFormatConverter>         m_bananaBitmap;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_whiteBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_headingTextFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_snappedViewFormat;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>           m_compositeModeTextLayout;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>           m_blendModeTextLayout;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_appleEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_bananaEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_compositeEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_blendEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_scaleEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_appleAlphaEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_bananaAlphaEffect;

    Platform::Array<Platform::String^>^                 m_compositeModeNames;
    Platform::Array<Platform::String^>^                 m_blendModeNames;
    D2D1_SIZE_U                                         m_bitmapPixelSize;
    D2D1_SIZE_F                                         m_imageSize;
    D2D1_SIZE_F                                         m_contextSize;
    D2D1_SIZE_F                                         m_cellSize;
    float                                               m_alpha;
    float                                               m_horizontalOffset;
    bool                                                m_drawEnabled;

    IDWriteTextLayout**                                 m_captionLayouts;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
