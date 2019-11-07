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
#include "Scenario4.xaml.h"
#include <collection.h>

using namespace SDKSample::SDKTemplate;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
        // IMPORTANT NOTE: call this only if you plan on polling several different URLs, and only
        // once after the user installs the app or creates a secondary tile
	TileUpdateManager::CreateTileUpdaterForApplication()->EnableNotificationQueue(true);

    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::SDKTemplate::Scenario4::StartTilePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto urisToPoll = ref new Vector<Uri^>();

	AddURIIfValid(urisToPoll, PollURL1->Text);
	AddURIIfValid(urisToPoll, PollURL2->Text);
	AddURIIfValid(urisToPoll, PollURL3->Text);
	AddURIIfValid(urisToPoll, PollURL4->Text);
	AddURIIfValid(urisToPoll, PollURL5->Text);
	            
    auto recurrence = static_cast<PeriodicUpdateRecurrence>(PeriodicRecurrence->SelectedIndex);

	if (urisToPoll->Size == 1)
    {
        TileUpdateManager::CreateTileUpdaterForApplication()->StartPeriodicUpdate(urisToPoll->GetAt(0), recurrence);
        rootPage->NotifyUser("Started polling " + urisToPoll->GetAt(0)->DisplayUri + ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType::StatusMessage);
    }
    else if (urisToPoll->Size > 1)
    {
        TileUpdateManager::CreateTileUpdaterForApplication()->StartPeriodicUpdateBatch(urisToPoll, recurrence);
        rootPage->NotifyUser("Started polling the specified URLs. Look at the application’s tile on the Start menu to see the latest update.", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("Specify a URL that returns tile XML to begin tile polling.", NotifyType::ErrorMessage);
    }
}

void SDKSample::SDKTemplate::Scenario4::StopTilePolling_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TileUpdateManager::CreateTileUpdaterForApplication()->StopPeriodicUpdate();
	rootPage->NotifyUser("Stopped polling", NotifyType::StatusMessage);
}

bool inline SDKSample::SDKTemplate::Scenario4::AddURIIfValid(IVector<Uri^>^ vectorToReceive, String^ polledUrl)
{
	if (polledUrl != "http://" && polledUrl != "") {
        vectorToReceive->Append(ref new Uri(polledUrl));
		return true;
    }
	return false;
}
