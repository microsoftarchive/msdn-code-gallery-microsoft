//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

ref class D2DEffectsOnPrimitives : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    // DirectXBase methods
    virtual void HandleDeviceLost() override;
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

public:
    // IFrameworkView methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    // Event handlers
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

    void CreatePrimitives();
    void CreateEffectGraph();

    SampleOverlay^                                      m_sampleOverlay;

    Platform::String^                                   m_string;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>           m_textFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_whiteBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_coloredBrush;

    Microsoft::WRL::ComPtr<ID2D1Bitmap1>                m_inputImage;
    Microsoft::WRL::ComPtr<ID2D1Effect>                 m_arithmeticCompositeEffect;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
