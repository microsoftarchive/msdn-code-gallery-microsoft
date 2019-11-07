//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "BasicTimer.h"
#include <deque>

#define _USE_MATH_DEFINES
#include <math.h>

ref class D2DInterpolationModes : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
internal:
    D2DInterpolationModes();

    void UpdateEffectMatrices();

    void D2DInterpolationModes::MenuItemSelected(Windows::UI::Popups::IUICommand^ command);

    // DirectX Base Methods.
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

public:
    // IFrameworkView Methods.
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    // Event Handlers.
    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender
        );

    void OnDisplayContentsInvalidated(
        _In_ Platform::Object^ sender
        );

    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnVisibilityChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::VisibilityChangedEventArgs^ args
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

    void OnManipulationUpdated(
        _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
        _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
        );

    // Declare event handlers for responding to touch gestures.
    void HandleManipulationUpdated(
        Windows::Foundation::Point position,
        Windows::Foundation::Point positionDelta,
        float zoomDelta
        );

    void HandlePointerPressed(
        Windows::UI::Input::PointerPoint^ point
        );

    void AddMenuItem(
        Windows::UI::Popups::PopupMenu^ popupMenu,
        Platform::String^ caption
        );

    inline const float Clamp(float v, float low, float high)
    {
        return (v < low) ? low : (v > high) ? high : v;
    }

    Platform::Agile<Windows::UI::Input::GestureRecognizer> m_gestureRecognizer;

    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_whiteBrush;

    Platform::String^                            m_modeTextLeft;
    Platform::String^                            m_modeTextRight;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>    m_modeTextFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>    m_renderTimeFormat;

    float                                        m_totalZoom;
    float                                        m_imageRotate;
    D2D1_POINT_2F                                m_viewPosition;
    D2D1_SIZE_U                                  m_imageSize;
    D2D1_SIZE_F                                  m_imageSizeDips;
    bool                                         m_isSnapped;

    SampleOverlay^                               m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID2D1Effect>          m_bitmapSourceEffect;
    Microsoft::WRL::ComPtr<ID2D1Effect>          m_2DAffineTransformEffectLeft;
    Microsoft::WRL::ComPtr<ID2D1Effect>          m_2DAffineTransformEffectRight;

    // Used by the context menu to avoid code duplication.
    Microsoft::WRL::ComPtr<ID2D1Effect>          m_selectedEffect;

    // Measure time it takes to render each frame.
    BasicTimer^                                  m_renderTimer;
    float                                        m_renderDuration;

    bool                                         m_isWindowClosed;
    bool                                         m_isWindowVisible;
};

ref class DirectXAppSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};