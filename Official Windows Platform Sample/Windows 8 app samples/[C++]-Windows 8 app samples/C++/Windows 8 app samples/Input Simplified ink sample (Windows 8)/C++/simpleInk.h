// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once
#pragma warning (disable: 4451)  // warns about possible invalid marshaling of objects across contexts, however we don't need to worry about this because the classes in the Inking API are INoMarshal

#include "DirectXBase.h"

// A NOTE ON RENDERING
// We have two rendering modes: live rendering and Bezier (re-)rendering. Live rendering is active from the moment the 
// pointer makes contact to the moment it is released. In order to reduce latencies, in live rendering mode we interpolate
// inking samples with lines. Bezier rendering improves the quality of the stroke once the pointer is released by
// re-rendering it with Bezier curves interpolation.
// Live rendering begins with OnPointerPressed, is updated by OnPointerMoved and ends with OnPointerReleased.
// BezierRender, called by Render, renders all the strokes contained in the stroke container using Bezier curves.
//
// In the interest of minimizing lagging between the rendered stroke and the position of the inking device,
// we shall use immediate-mode presentation (Present(0, ...)) while in live rendering. This way we can always present
// the most up to date inking sample. However immediate mode presentation drains the battery so we shall switch back
// to non-immediate mode presentation (Present(1, ...)) as soon as we exit live rendering.

typedef enum {
    Bezier,
    Live
} RenderingMode;

ref class simpleInk sealed : public DirectXBase, public Windows::ApplicationModel::Core::IFrameworkView
{
public:
    simpleInk();

    // DirectXBase Methods
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Present() override;
    virtual void Render() override;

    // IFrameworkView Methods
    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(_In_ Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

protected:
    // Event Handlers
    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args);
    void OnLogicalDpiChanged(_In_ Platform::Object^ sender);
    void OnDisplayContentsInvalidated(_In_ Platform::Object^ sender);
    void OnActivated(
        _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView, 
        _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);
    void OnCharacterReceived(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::CharacterReceivedEventArgs^ args);
    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerMoved(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerReleased(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::PointerEventArgs^ args);

    void OnCopyToClipboard();
    void OnCopyTextToClipboard();
    void OnPasteFromClipboard();
    void OnSave();
    void OnLoad();
    void OnSelectAll();
    void OnDelete();
    void OnChangeDrawingAttributes();
    void OnChangeRecognizer();
    void OnRecognize();

    void BezierRender();

internal:
    void ConvertStrokeToGeometry(
        _In_ Windows::UI::Input::Inking::InkStroke^ stroke,
        _Outptr_ ID2D1PathGeometry** geometry);

private:
    // D2D data members
    Microsoft::WRL::ComPtr<ID3D11Texture2D> _currentBuffer;
    Microsoft::WRL::ComPtr<ID3D11Texture2D> _previousBuffer;
    Microsoft::WRL::ComPtr<IDWriteTextFormat> _eventTextFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _backgroundBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _messageBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _inkBrush;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> _selectionBrush;
    Microsoft::WRL::ComPtr<ID2D1StrokeStyle> _inkStyle;
    Microsoft::WRL::ComPtr<ID2D1StrokeStyle> _selectionStyle;

    Windows::UI::Color _backgroundColor;

    // Inking members
    Windows::UI::Input::Inking::InkStrokeBuilder^ _strokeBuilder;
    Windows::UI::Input::Inking::InkStrokeContainer^ _strokeContainer;
    Windows::UI::Input::Inking::InkRecognizerContainer^ _recognizerContainer;
    Windows::UI::Input::Inking::InkRecognizer^ _recognizer;
    Windows::UI::Input::Inking::InkDrawingAttributes^ _drawingAttributes;

    unsigned int _recognizerId;
    Platform::String^ _recognitionText;
    Platform::String^ _statusMessage;

    // Stores the id of the 'active' pointer, -1 if none. We are allowing only 
    // one 'active' pointer at a time. This variable is set by OnPointerPressed,
    // checked by OnPointerMoved, and reset by OnPointerReleased.
    int _pointerId;

    Windows::Foundation::Collections::IVector<Windows::Foundation::Point>^ _manipulationPoints;
    Windows::UI::Input::Inking::InkManipulationMode _manipulationMode;

    RenderingMode _renderingMode;
};

ref class DirectXAppSource sealed : Windows::ApplicationModel::Core::IFrameworkViewSource
{
public:
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();
};

#pragma warning (default: 4451)
