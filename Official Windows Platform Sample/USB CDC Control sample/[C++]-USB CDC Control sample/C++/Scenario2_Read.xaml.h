//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_Read.xaml.h
// Declaration of the CdcAcmRead class.
//

#pragma once

#include "Scenario2_Read.g.h"
#include "SerialPort.h"

namespace SDKSample
{
    namespace UsbCdcControl
    {
        /// <summary>
        /// Page containing working sample code to demonstrate how to read data from a CDC ACM device.
        /// </summary>
        public ref class CdcAcmRead sealed
        {
        public:
            CdcAcmRead();

        protected:
            virtual void LoadState(Platform::Object^ navigationParameter,
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info);
            Windows::Foundation::IAsyncAction^ OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info);
            void ReadByteOneByOne();
            Windows::Foundation::IAsyncOperation<int>^ Read(Windows::Storage::Streams::Buffer^ buffer, int timeout);

        private:
            Windows::Foundation::EventRegistrationToken onDeviceAddedRegToken;
            Windows::Foundation::EventRegistrationToken onDeviceRemovedRegToken;
            Windows::Foundation::IAsyncOperation<int>^ opRead;

            void buttonReadBulkIn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e);
            void buttonWatchBulkIn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonStopWatching_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void radioButtonDataFormat_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}