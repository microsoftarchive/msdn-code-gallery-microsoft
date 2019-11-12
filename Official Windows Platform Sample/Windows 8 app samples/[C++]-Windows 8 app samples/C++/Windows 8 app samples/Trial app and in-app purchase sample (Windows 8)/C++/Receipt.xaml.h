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
// Receipt.xaml.h
// Declaration of the Receipt class
//

#pragma once

#include "pch.h"
#include "Receipt.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Receipt sealed
        {
        public:
            Receipt();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::EventRegistrationToken eventRegistrationToken;

            void LoadReceiptProxyFile();
            void ReceiptRefreshScenario();

            void ShowReceipt_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
