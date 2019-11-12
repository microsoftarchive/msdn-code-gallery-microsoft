//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "EffectsFlyout.g.h"

namespace Postcard
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public delegate void EffectIntensityChangedHandler(
        Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
        );

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class EffectsFlyout sealed
    {
    public:
        EffectsFlyout();

        event EffectIntensityChangedHandler^ EffectIntensityChanged;

        void EffectIntensitySliderChanged(
            Platform::Object^ sender,
            Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e
            );
    };
}
