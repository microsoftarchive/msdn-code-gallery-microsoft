//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4_Loopback.xaml.h
// Declaration of the CdcAcmLoopback class
//

#pragma once

#include "Scenario4_Loopback.g.h"
#include "SerialPort.h"

namespace SDKSample
{
    namespace UsbCdcControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class CdcAcmLoopback sealed
        {
        public:
            CdcAcmLoopback();

        protected:
            virtual void LoadState(Platform::Object^ navigationParameter,
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info);
            Windows::Foundation::IAsyncAction^ OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info);
            Windows::Foundation::IAsyncOperation<int>^ Read(UsbSerialPort^ port, Windows::Storage::Streams::Buffer^ buffer, int timeout);

            property UsbSerialPortInfo^ SerialPortInfo1
            {
                UsbSerialPortInfo^ get()
                {
                    if (UsbDeviceList::Singleton->List->Size > 0 && comboBoxDevices1->SelectedIndex != -1)
                    {
                        for (unsigned int i = 0; i < UsbDeviceList::Singleton->List->Size; i++)
                        {
                            if (UsbDeviceList::Singleton->List->GetAt(i)->DeviceId == ((UsbDeviceComboBoxItem^) comboBoxDevices1->SelectedItem)->Id)
                            {
                                return UsbDeviceList::Singleton->List->GetAt(i);
                            }
                        }
                    }
                    return nullptr;
                }
            }

            property UsbSerialPortInfo^ SerialPortInfo2
            {
                UsbSerialPortInfo^ get()
                {
                    if (UsbDeviceList::Singleton->List->Size > 0 && comboBoxDevices2->SelectedIndex != -1)
                    {
                        for (unsigned int i = 0; i < UsbDeviceList::Singleton->List->Size; i++)
                        {
                            if (UsbDeviceList::Singleton->List->GetAt(i)->DeviceId == ((UsbDeviceComboBoxItem^) comboBoxDevices2->SelectedItem)->Id)
                            {
                                return UsbDeviceList::Singleton->List->GetAt(i);
                            }
                        }
                    }
                    return nullptr;
                }
            }

        private:
            Windows::Foundation::EventRegistrationToken onDeviceAddedRegToken;
            Windows::Foundation::EventRegistrationToken onDeviceRemovedRegToken;
            Platform::String^ previousSelectedDeviceId1;
            Platform::String^ previousSelectedDeviceId2;
            Windows::Foundation::IAsyncOperation<int>^ opRead;
            Platform::Array<Windows::UI::Xaml::Controls::ComboBox^>^ comboBoxDevicesArray;
            Platform::String^ statusMessage;

            void comboBoxDevices_SelectionChanged(Platform::Object^, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^);
            void buttonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonLoopbackTest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonStopLoopback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e);
        };
    }
}