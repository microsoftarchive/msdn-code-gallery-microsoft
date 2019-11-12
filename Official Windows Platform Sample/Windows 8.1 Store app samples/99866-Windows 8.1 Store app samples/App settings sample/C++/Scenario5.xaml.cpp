// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample::ApplicationSettings;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

SDKSample::ApplicationSettings::Scenario5::Scenario5()
{
	InitializeComponent();
}

void SDKSample::ApplicationSettings::Scenario5::showSettingsFlyout_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = dynamic_cast<Button^>(sender);
	if (b != nullptr)
	{
		auto sf = ref new SettingsFlyout2();
		sf->ShowIndependent();
		MainPage::Current->NotifyUser("You opened a SettingsFlyout with layered content", NotifyType::StatusMessage);
	}
}
