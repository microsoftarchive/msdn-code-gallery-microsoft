//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

ref class CommandListRenderer : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    CommandListRenderer();

    // DirectXBase Methods
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

    void DrawPattern();
    void Update(float timeTotal);

    // Declare event handlers for responding to touch gestures
    void HandlePointerDoubleTapped(Windows::Foundation::Point position);

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

    // Declare GestureRecognizer Event Handlers
    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    void OnTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::TappedEventArgs^ args
        );

    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args
        );

    inline static float Clamp(float value, float low, float high)
    {
        return (value < low) ? low : (value > high) ? high : value;
    }

    inline static float LinearInterpolate(float start, float end, float weight)
    {
        return start * (1.0f - weight) + end * weight;
    }

    SampleOverlay^                                                  m_sampleOverlay;
    Microsoft::WRL::ComPtr<ID2D1ImageBrush>                         m_imageBrush;
    Microsoft::WRL::ComPtr<ID2D1CommandList>                        m_commandList;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                    m_blueBrush;

    Windows::Foundation::Point                                      m_viewPosition;
    FLOAT                                                           m_zoom;
    BOOL                                                            m_recenter;
    D2D1_POINT_2F                                                   m_recenterStartPosition;
    FLOAT                                                           m_recenterStartZoom;
    FLOAT                                                           m_recenterStartTime;
    Platform::Agile<Windows::UI::Input::GestureRecognizer>          m_gestureRecognizer;
    bool                                                            m_windowClosed;
    bool                                                            m_windowVisible;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
