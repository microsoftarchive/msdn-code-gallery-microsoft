//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME {
            Platform::String^ get() {  return ref new Platform::String(L"Push and Periodic Notifications"); }
        }

        static property Platform::Array<Scenario>^ scenarios {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

		property Windows::Networking::PushNotifications::PushNotificationChannel^ Channel {
			Windows::Networking::PushNotifications::PushNotificationChannel^ get()
			{
				return channel;
			}

			void set(Windows::Networking::PushNotifications::PushNotificationChannel^ newChannel)
			{
				channel = newChannel;
			}
		}


		property PushNotificationsHelper::Notifier^ Notifier {
			PushNotificationsHelper::Notifier^ get()
			{
				return notifier;
			}

			void set(PushNotificationsHelper::Notifier^ newNotifier)
			{
				notifier = newNotifier;
			}
		}
    private:
        static Platform::Array<Scenario>^ scenariosInner;
		Windows::Networking::PushNotifications::PushNotificationChannel^ channel;
		PushNotificationsHelper::Notifier^ notifier;
    };


}
