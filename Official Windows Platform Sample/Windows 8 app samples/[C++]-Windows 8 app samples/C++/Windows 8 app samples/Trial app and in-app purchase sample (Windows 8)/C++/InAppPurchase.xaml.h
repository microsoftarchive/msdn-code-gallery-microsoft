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
// InAppPurchase.xaml.h
// Declaration of the InAppPurchase class
//

#pragma once

#include "pch.h"
#include "InAppPurchase.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class InAppPurchase sealed
        {
        public:
            InAppPurchase();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::EventRegistrationToken eventRegistrationToken;

            void LoadInAppPurchaseProxyFile();
            void InAppPurchaseRefreshScenario();

            void TryProduct1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void BuyProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void TryProduct2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void BuyProduct2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}