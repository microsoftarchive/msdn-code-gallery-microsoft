// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario2_Manage.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Scenario2_Manage::Scenario2_Manage()
{
	InitializeComponent();
	scheduledNotificationsBinding = ref new NotificationDataSource();
    ItemGridView->ItemsSource = scheduledNotificationsBinding->Items;

    RefreshListButton->Click += ref new RoutedEventHandler(this, &Scenario2_Manage::RefreshList_Click);
    RemoveButton->Click += ref new RoutedEventHandler(this, &Scenario2_Manage::Remove_Click);
}

Scenario2_Manage::~Scenario2_Manage()
{
}

void Scenario2_Manage::RefreshList_Click(Object^ sender, RoutedEventArgs^ e)
{
    RefreshListView();
}

void Scenario2_Manage::Remove_Click(Object^ sender, RoutedEventArgs^ e)
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

void Scenario2_Manage::RefreshListView()
{
    scheduledNotificationsBinding->Refresh();
}


#pragma region Template-Related Code - Do not remove
void Scenario2_Manage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
	rootPage = MainPage::Current;
    RefreshListView();
}
#pragma endregion