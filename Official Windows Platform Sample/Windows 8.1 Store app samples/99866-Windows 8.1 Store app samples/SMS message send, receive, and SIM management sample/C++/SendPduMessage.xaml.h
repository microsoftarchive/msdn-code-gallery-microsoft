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
// SendPduMessage.xaml.h
// Declaration of the SendPduMessage class
//

#pragma once

#include "pch.h"
#include "SendPduMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsSendReceive
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class SendPduMessage sealed
        {
        public:
            SendPduMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
    		Windows::Devices::Sms::SmsDevice^ device;
    		void DoSend();
    		Platform::Array<unsigned char, 1U>^ ParseHexString(Platform::String^ messageText);
            void Send_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
