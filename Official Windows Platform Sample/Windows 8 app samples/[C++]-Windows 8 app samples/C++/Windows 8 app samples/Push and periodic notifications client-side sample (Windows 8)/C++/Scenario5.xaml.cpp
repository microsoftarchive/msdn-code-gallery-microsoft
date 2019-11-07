//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "Scenario5.xaml.h"

using namespace SDKSample::SDKTemplate;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::SDKTemplate::Scenario5::StartBadgePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	String^ polledUrl = BadgePollingURL->Text;

    // The default string for this text box is "http://".
    // Make sure the user has entered some data.
    if (polledUrl != "http://")
    {
        PeriodicUpdateRecurrence recurrence = static_cast<PeriodicUpdateRecurrence>(PeriodicRecurrence->SelectedIndex);

        // You can also specify a time you would like to start polling. Secondary tiles can also receive
        // polled updates using BadgeUpdateManager::CreateBadgeUpdaterForSecondaryTile(tileId).
        BadgeUpdateManager::CreateBadgeUpdaterForApplication()->StartPeriodicUpdate(ref new Uri(polledUrl), recurrence);

		rootPage->NotifyUser("Started polling " + polledUrl + ". Look at the application’s tile on the Start menu to see the latest update", NotifyType::StatusMessage);
    }
    else
    {
		rootPage->NotifyUser("Specify a URL that returns tile XML to begin Badge polling", NotifyType::ErrorMessage);
    }
}

void SDKSample::SDKTemplate::Scenario5::StopBadgePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TileUpdateManager::CreateTileUpdaterForApplication()->StopPeriodicUpdate();
	rootPage->NotifyUser("Stopped polling", NotifyType::StatusMessage);
}
