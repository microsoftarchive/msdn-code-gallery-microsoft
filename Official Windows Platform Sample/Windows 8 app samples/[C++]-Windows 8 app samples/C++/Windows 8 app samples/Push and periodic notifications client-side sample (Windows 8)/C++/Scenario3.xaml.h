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

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SDKTemplate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void AddCallback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void RemoveCallback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		bool UpdateListener(bool add);
    		void OnPushNotificationReceived(Windows::Networking::PushNotifications::PushNotificationChannel^ sender, Windows::Networking::PushNotifications::PushNotificationReceivedEventArgs^ e);
    		
    		Windows::Foundation::EventRegistrationToken pushReceivedToken;
        };
    }
}
