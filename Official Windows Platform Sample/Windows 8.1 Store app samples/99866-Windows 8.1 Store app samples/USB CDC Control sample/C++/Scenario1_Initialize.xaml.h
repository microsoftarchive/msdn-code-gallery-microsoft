//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_Initialize.xaml.h
// Declaration of the CdcAcmInitialize class.
//

#pragma once

#include "Scenario1_Initialize.g.h"
#include "SerialPort.h"

namespace SDKSample
{
    namespace UsbCdcControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class CdcAcmInitialize sealed
        {
        public:
            CdcAcmInitialize();

        protected:
            virtual void LoadState(Platform::Object^ navigationParameter,
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info);
            void AddDeviceToComboBox(UsbDeviceInfo^ info);
            Windows::Foundation::IAsyncAction^ OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info);
            void ReadByteOneByOne();
            Windows::Foundation::IAsyncOperation<int>^ Read(Windows::Storage::Streams::Buffer^ buffer, int timeout);

        private:
            Windows::Foundation::EventRegistrationToken onDeviceAddedRegToken;
            Windows::Foundation::EventRegistrationToken onDeviceRemovedRegToken;
            Platform::String^ previousSelectedDeviceId;

            void buttonDeviceSelect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonDeviceDeselect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}