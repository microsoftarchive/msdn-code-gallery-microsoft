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

using namespace SDKSample::ApplicationSettings;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

SDKSample::ApplicationSettings::Scenario3::Scenario3()
{
	InitializeComponent();
}

void SDKSample::ApplicationSettings::Scenario3::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	this->commandsRequestedEventRegistrationToken = SettingsPane::GetForCurrentView()->CommandsRequested += ref new TypedEventHandler<SettingsPane^, SettingsPaneCommandsRequestedEventArgs^>(this, &Scenario3::onCommandsRequested);
}

void SDKSample::ApplicationSettings::Scenario3::OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	SettingsPane::GetForCurrentView()->CommandsRequested -= this->commandsRequestedEventRegistrationToken;
}

void SDKSample::ApplicationSettings::Scenario3::onSettingsCommand(Windows::UI::Popups::IUICommand^ command)
{
	
	auto mySettings = ref new SettingsFlyout1();
	mySettings->Show();

	MainPage::Current->NotifyUser("You opened the 'Defaults' SettingsFlyout" , NotifyType::StatusMessage);
}

void SDKSample::ApplicationSettings::Scenario3::onCommandsRequested(SettingsPane^ settingsPane, SettingsPaneCommandsRequestedEventArgs^ e)
{
	UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler(this, &Scenario3::onSettingsCommand);

	SettingsCommand^ generalCommand = ref new SettingsCommand("defaults", "Defaults", handler);
	e->Request->ApplicationCommands->Append(generalCommand);
}

