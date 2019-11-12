// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Flyouts;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario3::Scenario3()
{
    InitializeComponent();
}

void Flyouts::Scenario3::listBox_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ListBox^ lb = dynamic_cast<ListBox^>(sender);
    if (lb != nullptr)
    {
		Flyout::ShowAttachedFlyout(lb);

		MainPage::Current->NotifyUser("Flyout opened from ListBox", NotifyType::StatusMessage);
    }
}
