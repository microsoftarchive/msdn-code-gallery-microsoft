//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_Write.xaml.h
// Declaration of the CdcAcmWrite class
//

#pragma once

#include "Scenario3_Write.g.h"

namespace SDKSample
{
    namespace UsbCdcControl
    {
        ref class BinaryDataControls
        {
        public:
            property Windows::UI::Xaml::Controls::Button^ Button;
            property Windows::UI::Xaml::Controls::TextBox^ TextBox;
        };

        /// <summary>
        /// A basic page that provides characteristics common to most applications.
        /// </summary>
        public ref class CdcAcmWrite sealed
        {
        private:
            static const Windows::UI::Xaml::DependencyProperty^ RawBinaryDataProperty;

        public:
            CdcAcmWrite();

        protected:
            virtual void LoadState(Platform::Object^ navigationParameter,
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info);
            Windows::Foundation::IAsyncAction^ OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info);

        private:
            Windows::Foundation::EventRegistrationToken onDeviceAddedRegToken;
            Windows::Foundation::EventRegistrationToken onDeviceRemovedRegToken;
            std::vector<BinaryDataControls^> binaryDataControls;

            void textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e);
            void buttonWriteBulkOut_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonSendBreak_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonWriteBinary_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void buttonLoadBinaryData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}