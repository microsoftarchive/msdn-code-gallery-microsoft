//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

enum class InkState
{
    StrokeDrawing,
    StrokeCompleted
};

ref class InkRenderer
{
internal:
    InkRenderer(void);

    void CreateDeviceIndependentResources(Microsoft::WRL::ComPtr<ID2D1Factory1> d2dFactory);
    void CreateDeviceResources(Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext);
    void CreateWindowSizeDependentResources(float dpi);

    void DrawInk();

    void StartSignature();
    void Reset();

    void OnPointerPressed(_In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args);
    void OnPointerReleased(_In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args);
    void OnPointerMoved(_In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args);

private:
    void DeleteAllStrokes();
    void ConvertStrokeToGeometry(
        _In_ Windows::UI::Input::Inking::IInkStroke^ stroke,
        _Outptr_result_maybenull_ ID2D1PathGeometry** geometry
        );

    // Resources shared with PostcardRenderer.
    Microsoft::WRL::ComPtr<ID2D1Factory1>                       m_d2dFactory;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>                  m_d2dContext;
    float                                                       m_dpi;

    // Resources specific to this renderer.
    Platform::Agile<Windows::UI::Input::Inking::InkManager>     m_inkManager;
    InkState                                                    m_inkState;
    uint32                                                      m_pointerId;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                m_blackBrush;
    std::vector<D2D1_POINT_2F>                                  m_currentStroke;
    std::vector<Microsoft::WRL::ComPtr<ID2D1PathGeometry>>      m_strokes;
};

