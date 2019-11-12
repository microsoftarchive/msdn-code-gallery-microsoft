//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "FPSCounter.h"

#include "GeometryRealizationPackage.h"

ref class GeometryRealizationSample : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    GeometryRealizationSample();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void UpdateForWindowSizeChange() override;
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

    void CreateGeometries();
    void DiscardGeometries();
    void RenderMainContent(float time);
    void ShowMenu(Windows::Foundation::Point position);
    void UpdateGeometryRendering(Windows::UI::Popups::IUICommand^ command);

    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnHolding(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::HoldingEventArgs^ args
        );

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    SampleOverlay^ m_sampleOverlay;
    void UpdateDisplayText();

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_blackBrush;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>                       m_textFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_solidColorBrush;
    Microsoft::WRL::ComPtr<ID2D1Geometry>                           m_geometry;
    Microsoft::WRL::ComPtr<IGeometryRealization>                    m_realization;
    uint32                                                          m_numberOfSquares;
    int64                                                           m_pausedTime;
    int64                                                           m_timeDelta;
    FPSCounter^                                                     m_displayText;
    D2D1_ANTIALIAS_MODE                                             m_antialiasMode;
    bool                                                            m_useRealizations;
    bool                                                            m_drawStroke;
    bool                                                            m_paused;
    bool                                                            m_updateRealization;
    bool                                                            m_updateDisplayText;
    Platform::Agile<Windows::UI::Input::GestureRecognizer>          m_gestureRecognizer;
    bool                                                            m_windowClosed;
    bool                                                            m_windowVisible;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};

