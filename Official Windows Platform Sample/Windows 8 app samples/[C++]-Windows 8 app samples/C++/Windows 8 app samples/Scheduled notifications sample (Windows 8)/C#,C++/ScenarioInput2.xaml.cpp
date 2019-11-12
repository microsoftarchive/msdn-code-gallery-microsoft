// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.cpp
// Implementation of the Scenario2Input class.
//

#include "pch.h"
#include "ScenarioInput2.xaml.h"
#include "MainPage.xaml.h"

using namespace ScheduledNotificationsSampleCPP;

using namespace Platform;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput2::ScenarioInput2()
{
    InitializeComponent();
    scheduledNotificationsBinding = ref new NotificationDataSource();
    ItemGridView->ItemsSource = scheduledNotificationsBinding->Items;

    RefreshListButton->Click += ref new RoutedEventHandler(this, &ScenarioInput2::RefreshList_Click);
    RemoveButton->Click += ref new RoutedEventHandler(this, &ScenarioInput2::Remove_Click);
}

ScenarioInput2::~ScenarioInput2()
{
}

void ScenarioInput2::RefreshList_Click(Object^ sender, RoutedEventArgs^ e)
{
    RefreshListView();
}

void ScenarioInput2::Remove_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto items = ItemGridView->SelectedItems;
    int a = items->Size;
    for (unsigned int i = 0; i < items->Size; i++)
    {
        auto item = dynamic_cast<NotificationData^>(items->GetAt(i));
        String^ itemId = item->ItemId;
        if (item->get_IsTile()) 
        {
            auto updater = TileUpdateManager::CreateTileUpdaterForApplication();
            auto scheduled = updater->GetScheduledTileNotifications();
            for (unsigned int j = 0; j < scheduled->Size; j++) 
            {
                if (scheduled->GetAt(j)->Id == itemId) 
                {
                    updater->RemoveFromSchedule(scheduled->GetAt(j));
                }
            }
        }
        else 
        {
            auto notifier = ToastNotificationManager::CreateToastNotifier();
            auto scheduled = notifier->GetScheduledToastNotifications();
            for (unsigned int j = 0; j < scheduled->Size; j++)
            {
                if (scheduled->GetAt(j)->Id == itemId)
                {
                    try
                    {
                        notifier->RemoveFromSchedule(scheduled->GetAt(j));
                    }
                    catch (...)
                    {
                    }
                }
            }
        }
    }

    rootPage->NotifyUser("Removed selected scheduled notifications", NotifyType::StatusMessage);
    RefreshListView();
}

void ScenarioInput2::RefreshListView()
{
    scheduledNotificationsBinding->Refresh();
}


#pragma region Template-Related Code - Do not remove
void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
    RefreshListView();
}
#pragma endregion

