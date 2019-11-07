// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Flyouts;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario4::Scenario4()
{
	InitializeComponent();
	this->viewCount = 0;
}

void Flyouts::Scenario4::button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = dynamic_cast<Button^>(sender);
	if (b != nullptr)
	{
		MainPage::Current->NotifyUser("You opened a Flyout", NotifyType::StatusMessage);
	}
}

void Flyouts::Scenario4::flyout_Opening(Platform::Object^ sender, Platform::Object^ e)
{
	Flyout^ f = dynamic_cast<Flyout^>(sender);
	if (f != nullptr)
	{
		this->viewCount++;
		TextBlock^ tb = ref new TextBlock();
		tb->Text = "This Flyout has been opened " + this->viewCount + " time(s).";
		tb->Style = static_cast<Windows::UI::Xaml::Style^>(Application::Current->Resources->Lookup("BasicTextStyle"));
		f->Content = tb;
	}
}
