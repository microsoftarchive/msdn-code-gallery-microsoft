//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2.g.h"

namespace SDKSample
{
    namespace MagneticStripeReaderSample
    {
        /// <summary>
        /// Demonstrate how to use read AAMVA cards with MagneticStripeReader
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            Windows::Devices::PointOfService::MagneticStripeReader^ _reader;
            Windows::Devices::PointOfService::ClaimedMagneticStripeReader^ _claimedReader;
            Windows::Foundation::EventRegistrationToken _aamvaCardDataReceivedToken;
            Windows::Foundation::EventRegistrationToken _releaseDeviceRequestedToken;

            void ScenarioStartReadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ScenarioEndReadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            Concurrency::task<void> CreateDefaultReaderObject();
            Concurrency::task<void> ClaimReader();
            Concurrency::task<void> EnableReader();
            void UpdateReaderStatusTextBlock(Platform::String^ strMessage);
            void ResetTheScenarioState();

            void OnReleaseDeviceRequested(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedMagneticStripeReader ^args);

            void OnAamvaCardDataReceived(Windows::Devices::PointOfService::ClaimedMagneticStripeReader^sender, Windows::Devices::PointOfService::MagneticStripeReaderAamvaCardDataReceivedEventArgs^args);
        };
    }
}
