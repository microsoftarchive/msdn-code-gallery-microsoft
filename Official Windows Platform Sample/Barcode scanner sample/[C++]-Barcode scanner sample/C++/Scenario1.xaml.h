//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"
#include <ppl.h>
#include <ppltasks.h>

namespace SDKSample
{
    namespace BarcodeScannerCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
        
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Windows::Devices::PointOfService::BarcodeScanner^ scanner;
            Windows::Devices::PointOfService::ClaimedBarcodeScanner^ claimedScanner;
            Windows::Foundation::EventRegistrationToken dataReceivedToken;
            Windows::Foundation::EventRegistrationToken releaseDeviceRequestedToken;

            void ScenarioStartScanButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ScenarioEndScanButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            Concurrency::task<void> CreateDefaultScannerObject();
            Concurrency::task<void> ClaimScanner();
            Concurrency::task<void> EnableScanner();
            void UpdateOutput(Platform::String^ strMessage);
            void ResetTheScenarioState();

            void OnDataReceived(Windows::Devices::PointOfService::ClaimedBarcodeScanner ^sender, Windows::Devices::PointOfService::BarcodeScannerDataReceivedEventArgs ^args);
            void OnReleaseDeviceRequested(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedBarcodeScanner ^args);
        };
    }
}
