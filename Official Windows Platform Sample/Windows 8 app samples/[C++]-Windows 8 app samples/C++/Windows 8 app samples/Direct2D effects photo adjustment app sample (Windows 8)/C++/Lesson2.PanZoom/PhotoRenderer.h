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
#include "BasicTimer.h"

// Lesson 2:
// Control whether the application is running a realtime render loop, or whether
// it waits for events. The render loop is needed when running the pan/zoom recenter animation.
typedef enum class RenderingMode
{
    WaitForEvents,
    RunRecenterAnimation
};

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

    // Lesson 2:
    // Declare CoreWindow Event Handlers
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

    // Lesson 2:
    // Declare GestureRecognizer Event Handlers
    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    void OnTapped(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::TappedEventArgs^ args
        );

    inline static float Clamp(float v, float low, float high)
    {
        return (v < low) ? low : (v > high) ? high : v;
    }

    inline static float LinearInterpolate(float start, float end, float weight)
    {
        return start * (1.0f - weight) + end * weight;
    }

    inline static void ClampViewPosition(
        D2D1_POINT_2F *viewPosition,
        D2D1_SIZE_F contextSize,
        D2D1_SIZE_F imageSize,
        float zoom
        )
    {
        viewPosition->x = Clamp(
            viewPosition->x,
            contextSize.width - imageSize.width * zoom,
            0
            );

        viewPosition->y = Clamp(
            viewPosition->y,
            contextSize.height - imageSize.height * zoom,
            0
            );
    }

    void InitializeRecenterAnimation();
    void UpdateRecenterAnimation();
    void UpdatePanZoomEffectValues();

    SampleOverlay^                              m_sampleOverlay;

    // Lesson 2:
    // Declare the various member variables needed to maintain the state of
    // the touch manipulations. In the event of a reset to the center when the
    // user douple taps the screen, we have a set of recenter variables to
    // describe the animation.
    D2D1_POINT_2F                               m_viewPosition;
    float                                       m_zoom;

    // Lesson 2:
    // These variables store the animation state while the view is being reset.
    D2D1_POINT_2F                               m_recenterStartPosition;
    float                                       m_recenterStartZoom;
    BasicTimer^                                 m_timer;

    float                                       m_minZoom;
    D2D1_SIZE_U                                 m_bitmapPixelSize;
    D2D1_SIZE_F                                 m_imageSize;
    D2D1_SIZE_F                                 m_contextSize;
    bool                                        m_isWindowClosed;
    RenderingMode                               m_renderingMode;

    Microsoft::WRL::ComPtr<IWICFormatConverter> m_wicFormatConverter;
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_bitmapSourceEffect;

    // Lesson 2:
    // Declare the 2D affine transform that will perform the touch transforms.
    Microsoft::WRL::ComPtr<ID2D1Effect>         m_2dTransformEffect;

    // Lesson 2:
    // Declare the gesture recognizer
    Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
