//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario5_UsbDescriptors.xaml.cpp
// Implementation of the Scenario5_UsbDescriptors class
//

#include "pch.h"
#include "Scenario5_UsbDescriptors.xaml.h"
#include "MainPage.xaml.h"
#include <string>

using namespace Platform;
using namespace Platform::Collections;
using namespace Concurrency;
using namespace Windows::Devices::Usb;
using namespace Windows::Storage::Streams;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::CustomUsbDeviceAccess;

/// <summary>
/// Initializes the list of descriptors that can be displayed in the UI
/// </summary>
UsbDescriptors::UsbDescriptors(void)
{
    InitializeComponent();

    displayableDescriptorTypes = ref new Vector<DescriptorTypeEntry^>();
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("Device Descriptor", Descriptor::Device));
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("Configuration Descriptor", Descriptor::Configuration));
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("All Interface Descriptors", Descriptor::Interface));
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("All Endpoint Descriptors", Descriptor::Endpoint));
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("All Custom Descriptors", Descriptor::Custom));

    // The Product string is not a string descriptors, they are just strings
    // It is marked as String Descriptor because string descriptors are not available in this API. There
    // are, however, alternatives to getting the Product string.
    //
    // To get the Manufacturer, the Windows.Devices.Enumeration.Pnp API must be used. The usage of PnpObject is similar to that of the Enumeration API. When 
    // creating the PnpObject with one of it's static methods, add "System.Devices.Manufacturer" to the list of properties to include. Note that the PnpObjectType
    // must be of type DeviceContainer, otherwise the manufacturer name cannot be obtained.
    // Once the PnpObject for the device has been obtained, use PnpObject.Properties dictionary where the key is "System.Devices.Manufacturer" and the value is
    // the manufacturer name.
    displayableDescriptorTypes->Append(ref new DescriptorTypeEntry("Product String", Descriptor::String));

}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
///
/// We will enable/disable parts of the UI if the device doesn't support it.
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UsbDescriptors::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    // We will disable the scenario that is not supported by the device.
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::All, GenericScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);

    // Display list of descriptors that can be printed
     if (EventHandlerForDevice::Current->IsDeviceConnected 
         && (Utilities::GetDeviceType(EventHandlerForDevice::Current->Device) != DeviceType::None))
     {
        ListOfDescriptorTypesSource->Source = displayableDescriptorTypes;
     }
}

/// <summary>
/// When an entry is selected from the list of available descriptors, we will print out the contents of the descriptor.
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
void UsbDescriptors::Descriptor_SelectChanged(Object^ sender, RoutedEventArgs^ /* eventArgs */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto listOfDescriptorTypes = static_cast<ListBox^>(sender);
        auto selection = static_cast<DescriptorTypeEntry^>(listOfDescriptorTypes->SelectedValue);
        auto descriptorType = selection->DescriptorType;

        PrintDescriptor(descriptorType);
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }

}

/// <summary>
/// Obtains the stringified form of the chosen descriptor and prints it.
/// </summary>
/// <param name="descriptorType"></param>
void UsbDescriptors::PrintDescriptor(Descriptor descriptorType)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        String^ descriptor = nullptr;

        switch (descriptorType)
        {
        case Descriptor::Device:
            descriptor = GetDeviceDescriptorAsString();
            break;
        case Descriptor::Configuration:
            descriptor = GetConfigurationDescriptorAsString();
            break;
        case Descriptor::Interface:
            descriptor = GetInterfaceDescriptorsAsString();
            break;
        case Descriptor::Endpoint:
            descriptor = GetEndpointDescriptorsAsString();
            break;
        case Descriptor::String:
            descriptor = GetProductName();
            break;
        case Descriptor::Custom:
            descriptor = GetCustomDescriptorsAsString();
            break;
        }

        if (descriptor != nullptr)
        {
            MainPage::Current->NotifyUser(descriptor, NotifyType::StatusMessage);
        }
    }
}

String^ UsbDescriptors::GetDeviceDescriptorAsString(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto deviceDescriptor = EventHandlerForDevice::Current->Device->DeviceDescriptor;
    
        content = "Device Descriptor\n"
            + "\nUsb Spec Number : " + Utilities::ConvertToHex16Bit(deviceDescriptor->BcdUsb)
            + "\nMax Packet Size (Endpoint 0) : " + deviceDescriptor->MaxPacketSize0.ToString()
            + "\nVendor ID : " + Utilities::ConvertToHex16Bit(deviceDescriptor->VendorId)
            + "\nProduct ID : " + Utilities::ConvertToHex16Bit(deviceDescriptor->ProductId)
            + "\nDevice Revision : " + Utilities::ConvertToHex16Bit(deviceDescriptor->BcdDeviceRevision)
            + "\nNumber of Configurations : " + deviceDescriptor->NumberOfConfigurations.ToString();
    }

    return content;
}

String^ UsbDescriptors::GetConfigurationDescriptorAsString(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto usbConfiguration = EventHandlerForDevice::Current->Device->Configuration;
        auto configurationDescriptor = usbConfiguration->ConfigurationDescriptor;

        content = "Configuration Descriptor\n"
            + "\nNumber of Interfaces : " + usbConfiguration->UsbInterfaces->Size.ToString()
            + "\nConfiguration Value : " + Utilities::ConvertToHex8Bit(configurationDescriptor->ConfigurationValue)
            + "\nSelf Powered : " + configurationDescriptor->SelfPowered.ToString()
            + "\nRemote Wakeup : " + configurationDescriptor->RemoteWakeup.ToString()
            + "\nMax Power (milliAmps) : " + configurationDescriptor->MaxPowerMilliamps.ToString();
    }

    return content;
}

/// <summary>
/// Will convert all the interfaces to a human readable format
/// </summary>
String^ UsbDescriptors::GetInterfaceDescriptorsAsString(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto interfaces = EventHandlerForDevice::Current->Device->Configuration->UsbInterfaces;

        content = "Interface Descriptors";

        for each (UsbInterface^ usbInterface in interfaces)
        {
            // Will use class/subclass/protocol values from the first interface setting (usually the selected one)
            UsbInterfaceDescriptor^ usbInterfaceDescriptor = usbInterface->InterfaceSettings->GetAt(0)->InterfaceDescriptor;

            content += "\n\nInterface Number : " + Utilities::ConvertToHex8Bit(usbInterface->InterfaceNumber)
                + "\nClass Code : 0x" + Utilities::ConvertToHex8Bit(usbInterfaceDescriptor->ClassCode)
                + "\nSubclass Code : 0x" + Utilities::ConvertToHex8Bit(usbInterfaceDescriptor->SubclassCode)
                + "\nProtocol Code: 0x" + Utilities::ConvertToHex8Bit(usbInterfaceDescriptor->ProtocolCode)
                + "\nNumber of Interface Settings : " + usbInterface->InterfaceSettings->Size.ToString()
                + "\nNumber of open Bulk In Endpoints : " + usbInterface->BulkInPipes->Size.ToString()
                + "\nNumber of open Bulk Out Endpoints : " + usbInterface->BulkOutPipes->Size.ToString()
                + "\nNumber of open Interrupt In Endpoints : " + usbInterface->InterruptInPipes->Size.ToString()
                + "\nNumber of open Interrupt Out Endpoints : " + usbInterface->InterruptOutPipes->Size.ToString();
        }

    }

    return content;
}

/// <summary>
/// We will print all the endpoint descriptors of the default interface
/// </summary>
String^ UsbDescriptors::GetEndpointDescriptorsAsString(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        auto usbInterface = EventHandlerForDevice::Current->Device->DefaultInterface;
        auto bulkInPipes = usbInterface->BulkInPipes;
        auto bulkOutPipes = usbInterface->BulkOutPipes;
        auto interruptInPipes = usbInterface->InterruptInPipes;
        auto interruptOutPipes = usbInterface->InterruptOutPipes;

        content = "Endpoint Descriptors for open pipes";

        // Print Bulk In Endpoint descriptors
        for each (UsbBulkInPipe^ bulkInPipe in bulkInPipes)
        {
            auto endpointDescriptor = bulkInPipe->EndpointDescriptor;

            content += "\n\nBulk In Endpoint Descriptor"
                + "\nEndpoint Number : " + Utilities::ConvertToHex8Bit(endpointDescriptor->EndpointNumber)
                + "\nMax Packet Size : " + endpointDescriptor->MaxPacketSize.ToString();
        }

        // Print Bulk Out Endpoint descriptors
        for each (UsbBulkOutPipe^ bulkOutPipe in bulkOutPipes)
        {
            auto endpointDescriptor = bulkOutPipe->EndpointDescriptor;
            
            content += "\n\nBulk Out Endpoint Descriptor"
                + "\nEndpoint Number : " + Utilities::ConvertToHex8Bit(endpointDescriptor->EndpointNumber)
                + "\nMax Packet Size : " + endpointDescriptor->MaxPacketSize.ToString();
        }

        // Print Interrupt In Endpoint descriptors
        for each (UsbInterruptInPipe^ interruptInPipe in interruptInPipes)
        {
            auto endpointDescriptor = interruptInPipe->EndpointDescriptor;

            content += "\n\nInterrupt In Endpoint Descriptor"
                + "\nEndpoint Number : " + Utilities::ConvertToHex8Bit(endpointDescriptor->EndpointNumber)
                + "\nMax Packet Size : " + endpointDescriptor->MaxPacketSize.ToString()
                + "\nInterval(milliseconds) : " + (endpointDescriptor->Interval.Duration / 10000).ToString();
        }

        // Print Interrupt Out Endpoint descriptors
        for each (UsbInterruptOutPipe^ interruptOutPipe in interruptOutPipes)
        {
            auto endpointDescriptor = interruptOutPipe->EndpointDescriptor;

            content += "\n\nInterrupt Out Endpoint Descriptor"
                + "\nEndpoint Number : " + Utilities::ConvertToHex8Bit(endpointDescriptor->EndpointNumber)
                + "\nMax Packet Size : " + endpointDescriptor->MaxPacketSize.ToString()
                + "\nInterval(milliseconds) : " + (endpointDescriptor->Interval.Duration / 10000).ToString();
        }
    }

    return content;
}

/// <summary>
/// It is recommended to obtain the product string by using DeviceInformation::Name.
/// </summary>
String^ UsbDescriptors::GetProductName(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        content = "Product name: " + EventHandlerForDevice::Current->DeviceInformation->Name;
    }

    return content;
}

/// <summary>
/// There are no custom descriptors for the SuperMutt and OSRFX2 devices, however, this method will show you how to
/// navigate through raw descriptors. The raw descriptors can be found in UsbConfiguration and
/// UsbInterface, and UsbInterfaceSetting. All possible descriptors that you would find in the full Configuration Descriptor
/// are found under UsbConfiguration.
/// All raw descriptors under UsbInterface are constrained only to those that are found under the InterfaceDescriptor
/// that is defined in the Usb spec.
///
/// Usb descriptor header (first 2 bytes):
/// bLength             Size of descriptor in bytes
/// bDescriptorType     The type of descriptor (configuration, interface, endpoint)
/// </summary>
String^ UsbDescriptors::GetCustomDescriptorsAsString(void)
{
    String^ content = nullptr;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        // Descriptor information will be appended to this string and then printed to UI
        content = "Raw Descriptors";
        
        auto configuration = EventHandlerForDevice::Current->Device->Configuration;
        auto allRawDescriptors = configuration->Descriptors;

        // Print first 2 bytes of all descriptors within the configuration descriptor    
        // because the first 2 bytes are always length and descriptor type
        // the UsbDescriptor's DescriptorType and Length properties, but we will not use these properties
        // in order to demonstrate ReadDescriptorBuffer() and how to parse it.
        for each (UsbDescriptor^ descriptor in allRawDescriptors)
        {
            auto descriptorBuffer = ref new Buffer(descriptor->Length);
            descriptor->ReadDescriptorBuffer(descriptorBuffer);
    
            DataReader^ reader = DataReader::FromBuffer(descriptorBuffer);

            // USB data is Little Endian according to the USB spec.
            reader->ByteOrder = ByteOrder::LittleEndian;    

            // ReadByte has a side effect where it consumes the current byte, so the next ReadByte will read the next character.
            // Putting multiple ReadByte() on the same line (same variable assignment) may cause the bytes to be read out of order.
            auto length = reader->ReadByte().ToString();
            auto type = Utilities::ConvertToHex8Bit(reader->ReadByte());

            content += "\n\nDescriptor"
                + "\nLength : " + length
                + "\nDescriptorType : " + type;
        }
    }

    return content;
}
