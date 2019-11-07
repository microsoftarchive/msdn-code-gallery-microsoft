//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.g.h"

namespace D2D3DTransforms
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
        void SetTransformEffect(TransformEffect transformEffect);
        void SetTransformProperty(TransformProperty transformProperty, float value);

        void OnEffectSelectorSelectionChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ args
            );

        void OnScaleXValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnScaleYValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnLocalOffsetXValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnLocalOffsetYValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnLocalOffsetZValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnRotationXValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnRotationYValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnRotationZValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnGlobalOffsetXValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnGlobalOffsetYValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnGlobalOffsetZValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnPerspectiveValueChanged(
            Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ args
            );

        void OnRestoreDefaultsClick(
            Object^ sender,
            Windows::UI::Xaml::RoutedEventArgs^ args
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

        D2D3DTransformsRenderer^ m_renderer;
    };
}
