//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_InterruptPipes.xaml.h
// Declaration of the InterruptPipes class
//

#pragma once
#include "Scenario3_InterruptPipes.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// This page demonstrates how to use interrupts pipes on a UsbDevice
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class InterruptPipes sealed
        {
        public:
            InterruptPipes(void);
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
        private:
            Windows::Foundation::EventRegistrationToken interruptEventRegistrationToken;
            uint32 registeredInterruptPipeIndex; // Pipe index of the pipe we that we registered for. Only valid if registeredInterrupt is true
            bool registeredInterrupt;

            uint32 numInterruptsReceived;
            uint32 totalNumberBytesReceived;
            uint32 totalNumberBytesWritten;

            // Did we navigate away from this page?
            bool navigatedAway;

            // Only valid for the OSRFX2 device
            Platform::Array<bool>^ previousSwitchStates;    

            void OnDeviceClosing(EventHandlerForDevice^ sender, Windows::Devices::Enumeration::DeviceInformation^ deviceInformation);

            void RegisterOsrFx2InterruptEvent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ eventArgs);
            
            void RegisterSuperMuttInterruptEvent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ eventArgs);
            void WriteSuperMuttInterruptOut_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ eventArgs);
            
            void UnregisterInterruptEvent_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ eventArgs);
            
            void RegisterForInterruptEvent(
                uint32 pipeIndex, 
                Windows::Foundation::TypedEventHandler<
                    Windows::Devices::Usb::UsbInterruptInPipe^, 
                    Windows::Devices::Usb::UsbInterruptInEventArgs^>^ eventHandler);

            void UnregisterFromInterruptEvent(void);

            void OnOsrFx2SwitchStateChangeEvent(
                Windows::Devices::Usb::UsbInterruptInPipe^ sender, 
                Windows::Devices::Usb::UsbInterruptInEventArgs^ eventArgs);

            void OnGeneralInterruptEvent(
                Windows::Devices::Usb::UsbInterruptInPipe^ sender,
                Windows::Devices::Usb::UsbInterruptInEventArgs^ eventArgs);

            void WriteToInterruptOut(uint32 pipeIndex, uint32 bytesToWrite);

            bool CreateBooleanChartInTable(
                Windows::UI::Xaml::Documents::InlineCollection^ table,
                const Platform::Array<bool>^ newValues,
                const Platform::Array<bool>^ oldValues,
                Platform::String^ trueValue,
                Platform::String^ falseValue);

            void ClearSwitchStateTable(void);
            void UpdateSwitchStateTable(Platform::Array<bool>^ states);

            void UpdateRegisterEventButton(void);
        };
    }
}
