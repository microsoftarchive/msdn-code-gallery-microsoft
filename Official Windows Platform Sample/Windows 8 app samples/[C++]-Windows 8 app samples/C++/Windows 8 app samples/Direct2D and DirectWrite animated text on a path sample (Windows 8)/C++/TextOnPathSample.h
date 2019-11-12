//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <string>
#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "PathTextRenderer.h"

ref class TextOnPath : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    TextOnPath();

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

    void OnVisibilityChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::VisibilityChangedEventArgs^ args
        );

    void OnLogicalDpiChanged(
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

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    void OnKeyPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CharacterReceivedEventArgs^ args
        );

    // Sample Methods
    void Update(float timeTotal);
    void CreateTextLayout();
    void CreateGeometry();
    void DiscardGeometry();
    void DiscardTextLayout();

    SampleOverlay^                                                  m_sampleOverlay;
    bool                                                            m_isSnapped;
    bool                                                            m_windowClosed;
    bool                                                            m_windowVisible;
    Microsoft::WRL::ComPtr<ID2D1PathGeometry>                       m_geometry;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_blackBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>                       m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>                       m_snappedViewFormat;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>                       m_textLayout;
    Microsoft::WRL::ComPtr<PathTextRenderer>                        m_textRenderer;
    std::wstring                                                    m_displayString;
    Microsoft::WRL::ComPtr<IDWriteRenderingParams>                  m_renderingParams;
    FLOAT                                                           m_currentTime;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
