//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "FlowLayout.h"

ref class DWriteVerticalText : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    DWriteVerticalText();

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

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_blackBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>                       m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTypography>                       m_textTypography;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>                       m_textLayout;
    Microsoft::WRL::ComPtr<IDWriteRenderingParams>                  m_renderingParams;
    Microsoft::WRL::ComPtr<FlowLayoutSource>                        m_flowLayoutSource;
    Microsoft::WRL::ComPtr<FlowLayoutSink>                          m_flowLayoutSink;
    Microsoft::WRL::ComPtr<FlowLayout>                              m_flowLayout;
    SampleOverlay^                                                  m_sampleOverlay;
    const wchar_t*                                                  m_fontName;
    const wchar_t*                                                  m_localeName;
    UINT32                                                          m_textLength;
    float                                                           m_fontSize;
    const wchar_t*                                                  m_text;
    ReadingDirection                                                m_readingDirection;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
