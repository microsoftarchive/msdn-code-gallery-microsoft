//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.g.h"
#include "D2DLightingEffectsRenderer.h"

namespace D2DLightingEffects
{
    public ref class MainPage sealed
    {
    public:
        MainPage();

        void InitializeValues();
        void ResetValues();

        void OnSuspending(
            Object^ sender,
            Windows::ApplicationModel::SuspendingEventArgs^ args
            );

    private:
        void SetLightingEffect(LightingEffect lightingEffect);
        void SetLightingProperty(LightingProperty lightingProperty, float value);

        void OnEffectSelectorSelectionChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e
            );

        void OnLightPositionZValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnSpecularConstantValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnSpecularExponentValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnDiffuseConstantValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnSpotFocusValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnLimitingConeAngleValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnAzimuthValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnElevationValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnSurfaceScaleValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );

        void OnRestoreDefaultsClick(
            Object^ sender,
            Windows::UI::Xaml::RoutedEventArgs^ e
            );

        void OnSwapChainPointerMoved(
            Object^ sender,
            Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
            );

        void OnLogicalDpiChanged(
            Object^ sender
            );

        void OnDisplayContentsInvalidated(
            Object^ sender
            );

        void OnWindowSizeChanged(
            Windows::UI::Core::CoreWindow^ sender,
            Windows::UI::Core::WindowSizeChangedEventArgs^ args
            );

        D2DLightingEffectsRenderer^ m_renderer;
    };
}
