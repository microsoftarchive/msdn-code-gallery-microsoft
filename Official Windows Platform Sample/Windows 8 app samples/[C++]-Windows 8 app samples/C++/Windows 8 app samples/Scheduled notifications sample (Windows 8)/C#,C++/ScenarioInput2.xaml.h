// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.h
// Declaration of the Scenario2Input class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput2.g.h"
#include "MainPage.g.h"
#include "NotificationData.h"

namespace ScheduledNotificationsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput2 sealed
    {
    public:
        ScenarioInput2();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioInput2();
        MainPage^ rootPage;
        NotificationDataSource^ scheduledNotificationsBinding;
        void Remove_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RefreshList_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RefreshListView();
    };
}
