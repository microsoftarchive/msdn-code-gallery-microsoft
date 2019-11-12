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
// ReadMessage.xaml.h
// Declaration of the ReadMessage class
//

#pragma once

#include "pch.h"
#include "ReadMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsSendReceive
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class ReadMessage sealed
        {
        public:
            ReadMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
    		Windows::Devices::Sms::SmsDevice^ device;
    
            void Read_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void DoReadMessage();
        };
    }
}
