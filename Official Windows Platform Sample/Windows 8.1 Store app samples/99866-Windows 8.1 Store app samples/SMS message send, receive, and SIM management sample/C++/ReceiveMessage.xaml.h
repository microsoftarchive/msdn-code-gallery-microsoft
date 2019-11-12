//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ReceiveMessage.xaml.h
// Declaration of the ReceiveMessage class
//

#pragma once

#include "pch.h"
#include "ReceiveMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsSendReceive
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class ReceiveMessage sealed
        {
        public:
            ReceiveMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
    		Windows::UI::Core::CoreDispatcher^ sampleDispatcher;
    		Windows::Devices::Sms::SmsDevice^ device;
    		Windows::Foundation::EventRegistrationToken smsMessageReceivedToken;
            int msgCount;
            bool listening;
    		void AddListener();
    		void device_SmsMessageReceived(Windows::Devices::Sms::SmsDevice^ sender, Windows::Devices::Sms::SmsMessageReceivedEventArgs^ args);
            void Receive_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
