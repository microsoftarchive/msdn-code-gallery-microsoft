//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_ControlTransfer.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2_ControlTransfer.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// This page demonstrates how to use control transfers to communicate with the device.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ControlTransfer sealed
        {
        public:
            ControlTransfer(void);
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
        private:
            void GetOsrFx2SevenSegmentSetting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SetOsrFx2SevenSegmentSetting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void GetSuperMuttLedBlinkPattern_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SetSuperMuttLedBlinkPattern_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
            Concurrency::task<void> SetOsrFx2SevenSegmentAsync(uint8 numericValue);
            Concurrency::task<void> GetOsrFx2SevenSegmentAsync(void);

            Concurrency::task<void> SetSuperMuttLedBlinkPatternAsync(uint8 pattern);
            Concurrency::task<void> GetSuperMuttLedBlinkPatternAsync(void);

            Concurrency::task<Windows::Storage::Streams::IBuffer^> SendVendorControlTransferInToDeviceRecipientAsync(
                uint8 vendorCommand, 
                uint32 dataPacketLength);
        };
    }
}
