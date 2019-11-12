//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "PaperFlyout.g.h"

namespace Postcard
{
    public delegate void ButtonClickedHandler(void);

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PaperFlyout sealed
    {
    public:
        PaperFlyout();

        event ButtonClickedHandler^ AddPaperClicked;
        event ButtonClickedHandler^ RemovePaperClicked;
        event ButtonClickedHandler^ MovePaperClicked;
        event ButtonClickedHandler^ StampPaperClicked;

    private:
        void AddButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RemoveButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void MoveButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void StampButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void Hide();
    };
}
