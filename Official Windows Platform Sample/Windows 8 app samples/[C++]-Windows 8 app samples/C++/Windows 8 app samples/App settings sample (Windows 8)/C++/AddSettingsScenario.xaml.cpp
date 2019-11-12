//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// AddSettingsScenario.xaml.cpp
// Implementation of the AddSettingsScenario class
//

#include "pch.h"
#include "AddSettingsScenario.xaml.h"

using namespace SDKSample::ApplicationSettings;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Popups;

AddSettingsScenario::AddSettingsScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void AddSettingsScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    rootPage->NotifyUser("To show the settings charm window, invoke the charm bar by swiping your finger on the right edge of the screen or bringing your mouse to the lower right corner of the screen, then select Settings. Or you can just press Windows logo + i. To dismiss the settings charm, tap in the application, swipe a screen edge, right click, invoke another charm or application.", NotifyType::StatusMessage);
}

void AddSettingsScenario::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Added to make sure the event handler for CommandsRequested in cleaned up before other scenarios
    if (this->isEventRegistered)
    {
        SettingsPane::GetForCurrentView()->CommandsRequested -= this->commandsRequestedEventRegistrationToken;
        this->isEventRegistered = false;
    }
}

void SDKSample::ApplicationSettings::AddSettingsScenario::addSettingsScenarioAdd_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        rootPage->NotifyUser("You selected the " + b->Content + " button", NotifyType::StatusMessage);
        if (!this->isEventRegistered)
        {
            // Listening for this event lets the app initialize the settings commands and pause its UI until the user closes the pane.
            // To ensure your settings are available at all times in your app, place your CommandsRequested handler in the overridden
            // OnWindowCreated of App.xaml.cpp
            this->commandsRequestedEventRegistrationToken = SettingsPane::GetForCurrentView()->CommandsRequested += ref new TypedEventHandler<SettingsPane^, SettingsPaneCommandsRequestedEventArgs^>(this, &AddSettingsScenario::onCommandsRequested);
            this->isEventRegistered = true;
        }
    }
}

void SDKSample::ApplicationSettings::AddSettingsScenario::onSettingsCommand(Windows::UI::Popups::IUICommand^ command)
{
    SettingsCommand^ settingsCommand = safe_cast<SettingsCommand^>(command);
    rootPage->NotifyUser("You selected the " + settingsCommand->Label + " settings command which originated from the " + SettingsPane::Edge.ToString(), NotifyType::StatusMessage);
}

void SDKSample::ApplicationSettings::AddSettingsScenario::onCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane^ settingsPane, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs^ eventArgs)
{
    UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler(this, &AddSettingsScenario::onSettingsCommand);

    SettingsCommand^ generalCommand = ref new SettingsCommand("generalSettings", "General", handler);
    eventArgs->Request->ApplicationCommands->Append(generalCommand);

    SettingsCommand^ helpCommand = ref new SettingsCommand("helpPage", "Help", handler);
    eventArgs->Request->ApplicationCommands->Append(helpCommand);
}
