// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#include "DirectXBase.h"
#include "DrawingObject.h"
ref class BackgroundObject;

#define DRAWING_OBJECTS_COUNT 4

ref class GestureRecognizerSample sealed : public DirectXBase, public IRenderer
{
public:
    GestureRecognizerSample(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);

    void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    void Run();

    // IRenderer implementation.
    virtual void RequestRedraw();
    virtual Windows::Foundation::Size ViewSize();

    // DirectXBase base class overrides.
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void Render() override;

protected:
    // ICoreApplicationView handlers.
    void OnViewActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ sender, 
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);

    // Process lifetime management handlers and helpers.
    void OnSuspending(
        _In_ Platform::Object^ sender,
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args);
    void OnResuming(
        _In_ Platform::Object^ sender,
        _In_ Platform::Object^ args);
    Windows::Foundation::Collections::IPropertySet^ GetObjectProperties(_In_ Platform::String^ name);

    // ICoreWindow event handlers and helpers.
    void OnWindowActivated(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowActivatedEventArgs^ args);
    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args);
    void OnWindowClosed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CoreWindowEventArgs^ args);
    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerWheelChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);

    // DirectXBase helpers.
    Windows::UI::Core::CoreWindow^ GetWindow() { return m_window.Get(); }

    // Graphics handlers.
    void OnLogicalDpiChanged(
        _In_ Platform::Object^ sender);
    void OnDisplayContentsInvalidated(
        _In_ Platform::Object^ sender);

    // Scene helpers.
    void ResetScene();
    void ChangeZOrder(int iFirst);

internal:
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> GetDeviceContext() { return m_d2dContext; }
    Microsoft::WRL::ComPtr<IDWriteFactory> GetDWriteFactory() { return m_dwriteFactory; }

private:
	~GestureRecognizerSample();

    // Graphics data members
    // Device independent resources
    Microsoft::WRL::ComPtr<IDWriteTextFormat> _eventTextFormat;
    // Device dependent resources
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _whiteBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _objectBrush[(int)DrawingObject::Color::MaxCount]; // brush per color, for painting objects

    // Scene data members
    BackgroundObject^ _background;
    Platform::Array<DrawingObject^>^ _objects;
    int _zorder[DRAWING_OBJECTS_COUNT];
    bool _redraw;
    bool _done;

    // IGestureRecognizer data mambers
    Platform::Array<Windows::UI::Input::GestureRecognizer^>^ _gestureRecognizers; // 1 recognizer for each DrawingObject and 1 for BackgroundObject
};

ref class GestureRecognizerSampleFrameworkView sealed : public Windows::ApplicationModel::Core::IFrameworkView
{
public:
    GestureRecognizerSampleFrameworkView();

    // IFrameworkView methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    typedef enum class ActivationEntryPoint
    {
        Unknown,
        GestureRecognizerSample
    } ActivationEntryPoint;
    ActivationEntryPoint _activationEntryPoint;
    GestureRecognizerSample^ _application;
};

ref class GestureRecognizerSampleFrameworkViewSource : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};
