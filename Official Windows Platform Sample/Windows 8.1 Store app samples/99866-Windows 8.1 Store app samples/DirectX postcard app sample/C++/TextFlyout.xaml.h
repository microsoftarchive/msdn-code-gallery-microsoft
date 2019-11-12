//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "TextFlyout.g.h"

namespace Postcard
{
    public delegate void TextSubmittedHandler(Platform::String^);

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class TextFlyout sealed
    {
    public:
        TextFlyout();
        event TextSubmittedHandler^ TextSubmitted;

    private:
        void TextBoxKeyUp(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
        void GotFocusHandler(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LostFocusHandler(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
