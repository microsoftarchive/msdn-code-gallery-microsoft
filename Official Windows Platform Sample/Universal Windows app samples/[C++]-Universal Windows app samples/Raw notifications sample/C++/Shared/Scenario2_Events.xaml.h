// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario2_Events.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario2 sealed
	{
	public:
		Scenario2();
	
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		
	private:
		MainPage^ rootPage;
        Windows::UI::Core::CoreDispatcher^ _dispatcher;
		Windows::Foundation::EventRegistrationToken pushToken;

		void Scenario2AddListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Scenario2RemoveListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		
		bool UpdateListener(bool add);
		void OnPushNotificationReceived(Windows::Networking::PushNotifications::PushNotificationChannel^ sender, Windows::Networking::PushNotifications::PushNotificationReceivedEventArgs^ e);
	};
}
