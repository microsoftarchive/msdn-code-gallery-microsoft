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

using namespace SDKSample::ApplicationSettings;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

SDKSample::ApplicationSettings::Scenario4::Scenario4()
{
	InitializeComponent();
}

void SDKSample::ApplicationSettings::Scenario4::showDefaults_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Button^ b = dynamic_cast<Button^>(sender);
	if (b != nullptr)
	{
		auto sf = ref new SettingsFlyout1();
		sf->ShowIndependent();
		SDKSample::MainPage::Current->NotifyUser("You opened the 'Defaults' SettingsFlyout", NotifyType::StatusMessage);
	}
}
