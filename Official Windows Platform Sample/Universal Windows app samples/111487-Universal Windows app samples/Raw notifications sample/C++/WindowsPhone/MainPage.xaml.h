// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "MainPage.g.h"
#include "SampleConfiguration.h"

namespace SDKSample
{
	public enum class NotifyType
	{
		StatusMessage,
		ErrorMessage
	};

    /// <summary>
    /// MainPage holds the status block and frame in which scenario files are loaded.
    /// </summary>
    public ref class MainPage sealed
    {
    public:
        MainPage();		

		property Windows::Networking::PushNotifications::PushNotificationChannel^ Channel
		{
			Windows::Networking::PushNotifications::PushNotificationChannel^ get() { return _channel; }
			void set(Windows::Networking::PushNotifications::PushNotificationChannel^ newChannel) { _channel = newChannel; }
		}
		
	protected:		
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;		

	private:		
		void HardwareButtons_BackPressed(Object^ sender, Windows::Phone::UI::Input::BackPressedEventArgs^ e);	
		Windows::Networking::PushNotifications::PushNotificationChannel^ _channel;

	internal:
		static MainPage^ Current;
		void NotifyUser(Platform::String^ strMessage, NotifyType type);
    };
}
