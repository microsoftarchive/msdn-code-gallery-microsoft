//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "SignatureFlyout.g.h"

namespace Postcard
{
    public delegate void SignatureButtonClickedHandler(void);

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class SignatureFlyout sealed
    {
    public:
        SignatureFlyout();

        event SignatureButtonClickedHandler^ SignClicked;
        event SignatureButtonClickedHandler^ EraseClicked;

    private:
        void SignButtonCicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void EraseButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void Hide();
    };
}
