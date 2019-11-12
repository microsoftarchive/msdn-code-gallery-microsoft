//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "TextFlyout.xaml.h"

using namespace Postcard;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

TextFlyout::TextFlyout()
{
    InitializeComponent();
}

void TextFlyout::TextBoxKeyUp(Object^ sender, KeyRoutedEventArgs^ e)
{
    Popup^ parent = dynamic_cast<Popup^>(this->Parent);
    if (e->Key == VirtualKey::Enter)
    {
        TextSubmitted(InputTextBox->Text);
        parent->IsOpen = false;
    }
    else if (e->Key == VirtualKey::Escape)
    {
        parent->IsOpen = false;
    }
}

void TextFlyout::GotFocusHandler(Object^ sender, RoutedEventArgs^ e)
{
    InputTextBox->SelectAll();
}
