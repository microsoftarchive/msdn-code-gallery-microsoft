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
// SendMessage.xaml.h
// Declaration of the SendMessage class
//

#pragma once

#include "pch.h"
#include "SendMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsSendReceive
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class SendMessage sealed
        {
        public:
            SendMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		
    		void DoSend();
    		Windows::Devices::Sms::SmsDevice^ device;
            void Send_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
