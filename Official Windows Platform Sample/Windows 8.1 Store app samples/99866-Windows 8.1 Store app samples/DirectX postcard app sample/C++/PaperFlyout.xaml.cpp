//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PaperFlyout.xaml.h"

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

PaperFlyout::PaperFlyout()
{
    InitializeComponent();
}

void PaperFlyout::AddButtonClicked(Object^ sender, RoutedEventArgs^ e)
{
    AddPaperClicked();
}

void PaperFlyout::RemoveButtonClicked(Object^ sender, RoutedEventArgs^ e)
{
    RemovePaperClicked();
    Hide();
}

void PaperFlyout::MoveButtonClicked(Object^ sender, RoutedEventArgs^ e)
{
    MovePaperClicked();
    Hide();
}

void PaperFlyout::StampButtonClicked(Object^ sender, RoutedEventArgs^ e)
{
    StampPaperClicked();
    Hide();
}

void PaperFlyout::Hide()
{
    Popup^ parent = dynamic_cast<Popup^>(this->Parent);
    if (parent != nullptr)
    {
        parent->IsOpen = false;
    }
}
