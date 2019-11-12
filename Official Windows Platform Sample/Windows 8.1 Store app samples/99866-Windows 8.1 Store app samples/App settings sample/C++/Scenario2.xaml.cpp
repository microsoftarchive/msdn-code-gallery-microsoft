// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ApplicationSettings;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

Scenario2::Scenario2()
{
	InitializeComponent();
	MainPage::Current->NotifyUser("Swipe the right edge of the screen to invoke the Charms bar and select Settings.  Alternatively, press Windows+I.", NotifyType::StatusMessage);
}

void ApplicationSettings::Scenario2::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	this->commandsRequestedEventRegistrationToken = SettingsPane::GetForCurrentView()->CommandsRequested += ref new TypedEventHandler<SettingsPane^, SettingsPaneCommandsRequestedEventArgs^>(this, &Scenario2::onCommandsRequested);
}

void ApplicationSettings::Scenario2::OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
	SettingsPane::GetForCurrentView()->CommandsRequested -= this->commandsRequestedEventRegistrationToken;
}

void ApplicationSettings::Scenario2::onSettingsCommand(Windows::UI::Popups::IUICommand^ command)
{
	SettingsCommand^ settingsCommand = safe_cast<SettingsCommand^>(command);
	MainPage::Current->NotifyUser("You selected the " + settingsCommand->Label + " SettingsCommand." , NotifyType::StatusMessage);
}

void ApplicationSettings::Scenario2::onCommandsRequested(SettingsPane^ settingsPane, SettingsPaneCommandsRequestedEventArgs^ e)
{
	UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler(this, &Scenario2::onSettingsCommand);

	SettingsCommand^ generalCommand = ref new SettingsCommand("general", "General", handler);
	e->Request->ApplicationCommands->Append(generalCommand);

	SettingsCommand^ helpCommand = ref new SettingsCommand("help", "Help", handler);
	e->Request->ApplicationCommands->Append(helpCommand);
}
