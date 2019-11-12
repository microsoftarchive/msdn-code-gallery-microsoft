//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_ControlTransfer.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2_ControlTransfer.xaml.h"
#include "MainPage.xaml.h"

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

ControlTransfer::ControlTransfer(void)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
///
/// We will enable/disable parts of the UI if the device doesn't support it.
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ControlTransfer::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    // We will disable the scenario that is not supported by the device.
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::OsrFx2, OsrFx2Scenario);
    deviceScenarios->Insert(DeviceType::SuperMutt, SuperMuttScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);
}

void ControlTransfer::GetOsrFx2SevenSegmentSetting_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        ButtonGetOsrFx2SevenSegment->IsEnabled = false;

        // Re-enable get button after completing the get
        GetOsrFx2SevenSegmentAsync().then([this](task<void> getSevenSegmentTask)
            {
                ButtonGetOsrFx2SevenSegment->IsEnabled = true;

                // May throw exception if there was an error getting data from the device
                getSevenSegmentTask.get();
            });
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void ControlTransfer::SetOsrFx2SevenSegmentSetting_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        ButtonSetOsrFx2SevenSegment->IsEnabled = false;

        uint8 numericValue = static_cast<uint8>(OsrFx2SevenSegmentSettingInput->SelectedIndex);

        // Re-enable set button after completing the set
        SetOsrFx2SevenSegmentAsync(numericValue).then([this](task<void> setSevenSegmentTask)
            {
                ButtonSetOsrFx2SevenSegment->IsEnabled = true;

                // May throw exception if there is a problem sending data to the device
                setSevenSegmentTask.get();
            });
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void ControlTransfer::GetSuperMuttLedBlinkPattern_Click(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        ButtonGetSuperMuttLedBlinkPattern->IsEnabled = false;

        // Re-enable get button after completing the get
        GetSuperMuttLedBlinkPatternAsync().then([this](task<void> getLedTask)
            {
                ButtonGetSuperMuttLedBlinkPattern->IsEnabled = true;

                // May throw exception if there was a problem getting Led pattern
                getLedTask.get();   
            });
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void ControlTransfer::SetSuperMuttLedBlinkPattern_Click(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        ButtonSetSuperMuttLedBlinkPattern->IsEnabled = false;

        uint8 pattern = static_cast<uint8>(SuperMuttLedBlinkPatternInput->SelectedIndex);

        // Re-enable set button after completing the set
        SetSuperMuttLedBlinkPatternAsync(pattern).then([this](task<void> setLedTask)
            {
                ButtonSetSuperMuttLedBlinkPattern->IsEnabled = true;

                // May throw exception if there was an error reading
                setLedTask.get();   
            });

    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// Gets the current value of the seven segment display on the OSRFX2 device via control transfer. 
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled at the 
/// end of the task chain.
///
/// When the seven segment hex value is received, we attempt to match it with a known (has a numerical value 0-9) hex value.
///
/// Getting Seven Segment LED require setup packet:
/// bmRequestType:   type:       VENDOR
///                  recipient:  DEVICE
/// bRequest:        0xD4
/// wLength:         1
///
/// The SuperMutt has the following patterns:
/// 0 - LED always on
/// 1 - LED flash 2 seconds on, 2 off, repeat
/// 2 - LED flash 2 seconds on, 1 off, 2 on, 4 off, repeat
/// ...
/// 7 - 7 iterations of 2 on, 1 off, followed by 4 off, repeat
/// </summary>
/// <returns>A task that can be used to chain more methods after completing the scenario</returns>
task<void> ControlTransfer::GetOsrFx2SevenSegmentAsync(void)
{
    // We expect to receive 1 byte of data with our control transfer, which is the state of the seven segment
    auto receiveDataSize = 1;

    // The Seven Segment display value is only 1 byte
    return SendVendorControlTransferInToDeviceRecipientAsync(OsrFx2::VendorCommand::GetSevenSegment, receiveDataSize)
        .then([receiveDataSize](IBuffer^ dataBuffer) 
        {
            if (dataBuffer->Length == receiveDataSize)
            {
                DataReader^ reader = DataReader::FromBuffer(dataBuffer);
                    
                // The raw hex value representing the seven segment display
                uint8 rawValue = reader->ReadByte();

                // The LED can't display the value of 255, so if the value
                // we receive is not a number, we'll know by seeing if this value changed
                uint8 readableValue = 255;      

                // Find which numeric value has the segment hex value
                for(int i = 0; i < ARRAYSIZE(OsrFx2::SevenLedSegmentMask); i += 1) 
                {
                    if (rawValue == OsrFx2::SevenLedSegmentMask[i]) {
                        readableValue = (uint8) i;

                        break;
                    }
                }

                // Seven Segment cannot display value 255, the value we received isn't a number
                if (readableValue == 255)    
                {
                    MainPage::Current->NotifyUser("The segment display is not yet initialized", NotifyType::ErrorMessage);
                }
                else
                {
                    MainPage::Current->NotifyUser("The segment display value is " + readableValue.ToString(), NotifyType::StatusMessage);
                }
            }
            else
            {
                MainPage::Current->NotifyUser(
                    "Expected to read " + receiveDataSize.ToString() + " bytes, but got " + dataBuffer->Length.ToString(), 
                    NotifyType::ErrorMessage);
            }
        });

}

/// <summary>
/// Sets the seven segment display on the OSRFX2 device via control transfer
/// 
/// Before sending the data through the control transfer, the numeric value is converted into a hex value that is
/// bit masked. Different sections of the bit mask will turn on a different LEDs. Please refer to the OSRFX2 spec
/// to see the hex values per LED.
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be 
/// handled at the end of the task chain.
///
/// Setting Seven Segment LED require setup packet:
/// bmRequestType:   type:       VENDOR
///                  recipient:  DEVICE
/// bRequest:        0xDB
/// wLength:         1
/// </summary>
/// <param name="numericValue"></param>
/// <returns>A task that can be used to chain more methods after completing the scenario</returns>
task<void> ControlTransfer::SetOsrFx2SevenSegmentAsync(uint8 numericValue)
{
    DataWriter^ writer = ref new DataWriter();

    // Convert the numeric value into a 7 segment LED hex value and write it to a buffer.
    writer->WriteByte(OsrFx2::SevenLedSegmentMask[numericValue]);

    // The buffer with the data
    auto bufferToSend = writer->DetachBuffer();
   
    UsbSetupPacket^ setupPacket = ref new UsbSetupPacket();
    setupPacket->RequestType->Recipient = UsbControlRecipient::Device;
    setupPacket->RequestType->ControlTransferType = UsbControlTransferType::Vendor;
    setupPacket->Request = OsrFx2::VendorCommand::SetSevenSegment;
    setupPacket->Value = 0;
    setupPacket->Length = bufferToSend->Length;

    return create_task(EventHandlerForDevice::Current->Device->SendControlOutTransferAsync(setupPacket, bufferToSend))
        .then([numericValue, bufferToSend](uint32 bytesTransferred)
        {
            // Make sure we sent the correct number of bytes
            if (bytesTransferred == bufferToSend->Length)
            {
                MainPage::Current->NotifyUser(
                    "The segment display value is set to " + numericValue.ToString(), 
                    NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser(
                    "Error sending data. Sent bytes: " + bytesTransferred.ToString() 
                    + "; Tried to send : " + bufferToSend->Length, 
                    NotifyType::ErrorMessage);
            }
        });
}

/// <summary>
/// Sets up a UsbSetupPacket that will let the device know that we are trying to get the Led blink pattern on the SuperMutt via
/// control transfer. 
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should 
/// be handled at the end of the task chain.
/// 
/// The simplest way to obtain a byte from the buffer is by using a DataReader. DataReader provides a simple way
/// to read from buffers (e.g. can return bytes, strings, ints).
///
/// Do note the method that is used to send a control transfer. There are two methods to send a control transfer. 
/// One is to send data and the other is to get data.
/// </summary>
/// <returns>A task that can be used to chain more methods after completing the scenario</returns>
task<void> ControlTransfer::GetSuperMuttLedBlinkPatternAsync(void)
{
    // The blink pattern is 1 byte long, so we only need to retrieve 1 byte of data
    return SendVendorControlTransferInToDeviceRecipientAsync(SuperMutt::VendorCommand::GetLedBlinkPattern, 1).then([this](IBuffer^ buffer)
        {
            if (buffer->Length > 0)
            {
                // Easiest way to read from a buffer
                auto dataReader = DataReader::FromBuffer(buffer);

                auto pattern = dataReader->ReadByte();

                MainPage::Current->NotifyUser("The Led blink pattern is " + pattern.ToString(), NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser("ControlInTransfer returned 0 bytes", NotifyType::ErrorMessage);
            }
        });
}

/// <summary>
/// Initiates a control transfer to set the blink pattern on the SuperMutt's LED. 
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
/// at the end of the task chain.
///
/// Setting LED blink pattern require setup packet:
/// bmRequestType:   type:       VENDOR
///                  recipient:  DEVICE
/// bRequest:        0x03
/// wValue:          0-7 (any number in that range, inclusive)
/// wLength:         0
///
/// The Buffer is used to hold data that is meant to be sent over during the data phase.
/// The easiest way to write data to an IBuffer is to use a DataWriter. The DataWriter, when instantiated, 
/// creates a buffer internally. The buffer is of type IBuffer and can be detached from the writer, which gives us
/// the internal IBuffer.
/// </summary>
/// <param name="pattern">A number from 0-7. Each number represents a different blinking pattern</param>
/// <returns>A task that can be used to chain more methods after completing the scenario</returns>
task<void> ControlTransfer::SetSuperMuttLedBlinkPatternAsync(uint8 pattern)
{
    UsbSetupPacket^ setupPacket = ref new UsbSetupPacket();
    setupPacket->RequestType->Recipient = UsbControlRecipient::Device;
    setupPacket->RequestType->ControlTransferType = UsbControlTransferType::Vendor;
    setupPacket->Request = SuperMutt::VendorCommand::SetLedBlinkPattern;
    setupPacket->Value = pattern;
    setupPacket->Length = 0;

    return create_task(EventHandlerForDevice::Current->Device->SendControlOutTransferAsync(setupPacket))
        .then([this, pattern](uint32 bytesTransferred)
        {
            MainPage::Current->NotifyUser(
                "The Led blink pattern is set to " + pattern.ToString(), 
                NotifyType::StatusMessage);
        });
}

/// <summary>
/// Sets up a UsbSetupPacket and sends control transfer that will let the device know that we are trying to retrieve data from the device.
/// This method only supports vendor commands because the scenario only uses vendor commands.
///
/// Do note the method that is used to send a control transfer. There are two methods to send a control transfer. 
/// One involves receiving buffer data in the Data stage of the control transfer, and the other involves transmiting the buffer data.
/// 
/// The simplest way to obtain a byte from the buffer is by using a DataReader. DataReader provides a simple way
/// to read from buffers (e.g. can return bytes, strings, ints).
/// </summary>
/// <param name="vendorCommand">Command to put into SetupPacket's Request property</param>
/// <param name="dataPacketLength">Number of bytes in the data packet that is sent in the Data Stage</param>
/// <returns>A task that can be used to chain more methods after completing the scenario</returns>
task<IBuffer^> ControlTransfer::SendVendorControlTransferInToDeviceRecipientAsync(uint8 vendorCommand, uint32 dataPacketLength)
{
    // Data will be written to this buffer when we receive it
    Buffer^ buffer = ref new Buffer(dataPacketLength);

    UsbSetupPacket^ setupPacket = ref new UsbSetupPacket();
    setupPacket->RequestType->Recipient = UsbControlRecipient::Device;
    setupPacket->RequestType->ControlTransferType = UsbControlTransferType::Vendor;
    setupPacket->Request = vendorCommand;
    setupPacket->Length = dataPacketLength;

    return create_task(EventHandlerForDevice::Current->Device->SendControlInTransferAsync(setupPacket, buffer));
}
