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
// InAppPurchaseConsumablesAdvanced.xaml.h
// Declaration of the InAppPurchaseConsumablesAdvanced class
//

#pragma once

#include "pch.h"
#include "InAppPurchaseConsumablesAdvanced.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class InAppPurchaseConsumablesAdvanced sealed
        {
        public:
            InAppPurchaseConsumablesAdvanced();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;

            void LoadInAppPurchaseConsumablesAdvancedProxyFile();

            void GetUnfulfilledConsumables();
            void GetUnfulfilledButton1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void GetUnfulfilledButton2_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void BuyProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void BuyProduct2Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void FulfillProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void FulfillProduct2Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void Log(Platform::String ^message, NotifyType type);
            void Log(Platform::String ^message, NotifyType type, int blankLines);

            void GrantFeatureLocally(Platform::String ^productId, Platform::Guid transactionId);
            bool IsLocallyFulfilled(Platform::Guid transactionId);

            Platform::Guid product1TempTransactionId;
            int product1NumPurchases;
            int product1NumFulfillments;

            Platform::Guid product2TempTransactionId;
            int product2NumPurchases;
            int product2NumFulfillments;

            Platform::Collections::Vector<Platform::Guid> ^consumedTransactionIds;
        };
    }
}