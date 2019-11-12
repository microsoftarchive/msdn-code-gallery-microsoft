//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario5_UsbDescriptors.xaml.h
// Declaration of the Scenario5_UsbDescriptors class
//

#pragma once
#include "Scenario5_UsbDescriptors.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// DescriptorTypeEntryClass is used so that the UI can bind to the list of these entries and display them.
        /// 
        /// This scenario can work for any device as long as it is "added" into the sample. For more information on how to add a 
        /// device to make it compatible with this scenario, please see Scenario1_DeviceConnect.
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        public ref class DescriptorTypeEntry sealed
        {
        public:
            property Platform::String^ EntryName
            {
                Platform::String^ get()
                {
                    return entryName;
                }
            }

            property Descriptor DescriptorType
            {
                Descriptor get()
                {
                    return descriptorType;
                }
            }

            DescriptorTypeEntry(Platform::String^ name, Descriptor type) :
                entryName(name), 
                descriptorType(type)
            {
            }

        private:
            Platform::String^ entryName;
            Descriptor descriptorType;
        };

        /// <summary>
        /// This scenario demonstrates how get various descriptors from UsbDevice
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class UsbDescriptors sealed
        {
        public:
            UsbDescriptors(void);
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
        private:
            Windows::Foundation::Collections::IObservableVector<DescriptorTypeEntry^>^ displayableDescriptorTypes;

            void Descriptor_SelectChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ eventArgs);

            void PrintDescriptor(CustomUsbDeviceAccess::Descriptor descriptorType);

            Platform::String^ GetDeviceDescriptorAsString(void);
            Platform::String^ GetConfigurationDescriptorAsString(void);
            Platform::String^ GetInterfaceDescriptorsAsString(void);
            Platform::String^ GetEndpointDescriptorsAsString(void);
            Platform::String^ GetCustomDescriptorsAsString(void);
            Platform::String^ GetProductName(void);
        };
    }
}
