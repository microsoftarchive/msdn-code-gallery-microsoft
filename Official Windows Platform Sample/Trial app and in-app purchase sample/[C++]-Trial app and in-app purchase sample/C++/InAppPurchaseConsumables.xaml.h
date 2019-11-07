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
// Declaration of the InAppPurchaseConsumables class
//

#pragma once

#include "pch.h"
#include "InAppPurchaseConsumables.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace StoreSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class InAppPurchaseConsumables sealed
        {
        public:
            InAppPurchaseConsumables();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;

            void LoadInAppPurchaseConsumablesProxyFile();
            void BuyAndFulfillProduct1Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void FulfillProduct1(Platform::String ^productId, Platform::Guid transactionId);

            void Log(Platform::String ^message, NotifyType type);

            void GrantFeatureLocally(Platform::Guid transactionId);
            bool IsLocallyFulfilled(Platform::Guid transactionId);

            int numberOfConsumablesPurchased;
            Platform::Collections::Vector<Platform::Guid> ^consumedTransactionIds;
        };
    }
}