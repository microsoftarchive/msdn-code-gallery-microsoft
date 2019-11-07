// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario2_Manage.g.h"
#include "MainPage.g.h"
#include "NotificationData.h"

namespace SDKSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario2_Manage sealed
	{
	public:
		Scenario2_Manage();
		
	protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~Scenario2_Manage();
        MainPage^ rootPage;
        NotificationDataSource^ scheduledNotificationsBinding;
        void Remove_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RefreshList_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RefreshListView();
	};
}
