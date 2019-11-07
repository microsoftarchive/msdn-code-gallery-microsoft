//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "PaperFragment.h"

enum class PaperMode
{
    Moving,
    Stamping,
    Deleting
};

ref class PaperRenderer
{
internal:
    PaperRenderer();

    void CreateDeviceIndependentResources(Microsoft::WRL::ComPtr<ID2D1Factory1> d2dFactory);
    void CreateDeviceResources(Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext);
    void CreateWindowSizeDependentResources(float dpi);

    void DrawPaper();

    void AddPaper();
    void RemovePaper();
    void MovePaper();
    void StampPaper();
    void Reset();

    void OnManipulationStarted(Windows::UI::Xaml::Input::ManipulationStartedRoutedEventArgs^ args);
    void OnManipulationCompleted(Windows::UI::Xaml::Input::ManipulationCompletedRoutedEventArgs^ args);
    void OnManipulationDelta(Windows::UI::Xaml::Input::ManipulationDeltaRoutedEventArgs^ args);
    void OnTapped(Windows::Foundation::Point position);

private:
    PaperFragment^ HitTestFragmentsWithPosition(D2D1_POINT_2F position);
    float RandomFloat();

    // Resources shared with PostcardRenderer.
    Microsoft::WRL::ComPtr<ID2D1Factory1>           m_d2dFactory;
    Microsoft::WRL::ComPtr<ID2D1DeviceContext>      m_d2dContext;
    float                                           m_dpi;

    // Resources specific to this renderer.
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_blackBrush;
    Microsoft::WRL::ComPtr<ID2D1Geometry>           m_starStampGeometry;
    std::list<PaperFragment^>                       m_paperFragmentList;
    PaperFragment^                                  m_selectedFragment;
    PaperMode                                       m_paperMode;
};
