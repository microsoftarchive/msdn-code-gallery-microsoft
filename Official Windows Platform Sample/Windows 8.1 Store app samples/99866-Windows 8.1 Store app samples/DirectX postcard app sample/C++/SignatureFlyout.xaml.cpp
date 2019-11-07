//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "SignatureFlyout.xaml.h"

using namespace Postcard;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

SignatureFlyout::SignatureFlyout()
{
    InitializeComponent();
}


void SignatureFlyout::SignButtonCicked(Object^ sender, RoutedEventArgs^ e)
{
    SignClicked();
    Hide();
}


void SignatureFlyout::EraseButtonClicked(Object^ sender, RoutedEventArgs^ e)
{
    EraseClicked();
    Hide();
}

void SignatureFlyout::Hide()
{
    Popup^ parent = dynamic_cast<Popup^>(this->Parent);
    if (parent != nullptr)
    {
        parent->IsOpen = false;
    }
}
