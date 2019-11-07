// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Flyouts;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario1::Scenario1()
{
	InitializeComponent();
}

void Flyouts::Scenario1::flyout_Opened(Platform::Object^ sender)
{
	Flyout^ f = dynamic_cast<Flyout^>(sender);
	if (f != nullptr)
	{
		MainPage::Current->NotifyUser("You opened a Flyout", NotifyType::StatusMessage);
	}
}

void Flyouts::Scenario1::menuFlyout_Opened(Platform::Object^ sender)
{
	MenuFlyout^ m = dynamic_cast<MenuFlyout^>(sender);
	if (m != nullptr)
	{
		MainPage::Current->NotifyUser("You opened a MenuFlyout", NotifyType::StatusMessage);
	}
}

void Flyouts::Scenario1::confirmPurchase_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Flyout^ f = dynamic_cast<Flyout^>(this->buttonWithFlyout->Flyout);
	f->Hide();

	MainPage::Current->NotifyUser("You bought an item!", NotifyType::StatusMessage);
}

void Flyouts::Scenario1::option_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	MenuFlyoutItem^ item = dynamic_cast<MenuFlyoutItem^>(sender);
	if (item != nullptr)
	{
		// A MenuFlyout closes automatically after the user selects an item
		MainPage::Current->NotifyUser("You selected option '" + item->Text + "'", NotifyType::StatusMessage);
	}
}

void Flyouts::Scenario1::enable_Click(Platform::Object^ sender)
{
	ToggleMenuFlyoutItem^ item = dynamic_cast<ToggleMenuFlyoutItem^>(sender);
	if (item != nullptr)
	{
		if (item->IsChecked)
		{
			MainPage::Current->NotifyUser("You enabled X", NotifyType::StatusMessage);
		}
		else 
		{
			MainPage::Current->NotifyUser("You disabled X", NotifyType::StatusMessage);
		}
	}
}
