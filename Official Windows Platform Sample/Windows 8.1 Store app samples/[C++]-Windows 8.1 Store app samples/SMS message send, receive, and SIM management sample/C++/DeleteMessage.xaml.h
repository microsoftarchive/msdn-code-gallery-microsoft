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
// DeleteMessage.xaml.h
// Declaration of the DeleteMessage class
//

#pragma once

#include "pch.h"
#include "DeleteMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace SmsSendReceive
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeleteMessage sealed
        {
        public:
            DeleteMessage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    
    		Windows::Devices::Sms::SmsDevice^ device;
    		void DeleteMessages(uint32 messageId);
    		void DoDelete(uint32 messageId);
            void Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DeleteAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
