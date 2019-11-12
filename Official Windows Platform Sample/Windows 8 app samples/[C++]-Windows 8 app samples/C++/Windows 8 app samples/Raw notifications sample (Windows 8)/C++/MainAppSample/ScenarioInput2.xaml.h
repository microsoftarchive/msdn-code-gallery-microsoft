// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput2.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace RawNotificationsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class ScenarioInput2 sealed
	{
	public:
		ScenarioInput2();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		~ScenarioInput2();
		MainPage^ rootPage;
        Windows::UI::Core::CoreDispatcher^ _dispatcher;
		Windows::Foundation::EventRegistrationToken pushToken;

		void Scenario2AddListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Scenario2RemoveListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		
		bool UpdateListener(bool add);
		void OnPushNotificationReceived(Windows::Networking::PushNotifications::PushNotificationChannel^ sender, Windows::Networking::PushNotifications::PushNotificationReceivedEventArgs^ e);
	};
}
