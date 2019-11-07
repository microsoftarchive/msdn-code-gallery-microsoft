//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_InterruptPipes.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_InterruptPipes.xaml.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Concurrency;
using namespace Windows::Foundation;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Usb;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::CustomUsbDeviceAccess;

InterruptPipes::InterruptPipes(void) :
    registeredInterrupt(false),
    registeredInterruptPipeIndex(0),
    numInterruptsReceived(0),
    totalNumberBytesReceived(0),
    totalNumberBytesWritten(0),
    previousSwitchStates(nullptr)
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
void InterruptPipes::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    navigatedAway = false;

    totalNumberBytesWritten = 0;

    // We will disable the scenario that is not supported by the device.
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::OsrFx2, OsrFx2Scenario);
    deviceScenarios->Insert(DeviceType::SuperMutt, SuperMuttScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);

    // Get notified when the device is about to be closed so we can unregister events
    EventHandlerForDevice::Current->OnDeviceClose = 
        ref new TypedEventHandler<EventHandlerForDevice^, DeviceInformation^>(this, &InterruptPipes::OnDeviceClosing);

    ClearSwitchStateTable();
    UpdateRegisterEventButton();
}

void InterruptPipes::OnNavigatedFrom(NavigationEventArgs^ /* eventArgs */) 
{
    navigatedAway = true;

    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        UnregisterFromInterruptEvent();
    }

    // We are leaving and no longer care about the device closing
    EventHandlerForDevice::Current->OnDeviceClose = nullptr;
}

void InterruptPipes::OnDeviceClosing(EventHandlerForDevice^ /* sender */, DeviceInformation^ /* deviceInformation */)
{
    UnregisterFromInterruptEvent();

    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
    {
        UpdateRegisterEventButton();
        ClearSwitchStateTable();
    }));
}

/// <summary>
/// This method will register for events on the correct interrupt in pipe index on the OsrFx2 device.
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
void InterruptPipes::RegisterOsrFx2InterruptEvent_Click(Object^ /* sender */, RoutedEventArgs^ /* eventArgs */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        uint32 interruptPipeIndex = OsrFx2::Pipe::InterruptInPipeIndex;
        TypedEventHandler<UsbInterruptInPipe^, UsbInterruptInEventArgs^>^ interruptEventHandler =
            ref new TypedEventHandler<UsbInterruptInPipe^, UsbInterruptInEventArgs^>(
                    this, 
                    &InterruptPipes::OnOsrFx2SwitchStateChangeEvent);

        RegisterForInterruptEvent(interruptPipeIndex, interruptEventHandler);

        UpdateRegisterEventButton();
        ClearSwitchStateTable();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// This method will register for events on the correct interrupt in pipe index on the OsrFx2 device.
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
void InterruptPipes::RegisterSuperMuttInterruptEvent_Click(Object^ /* sender */, RoutedEventArgs^ /* eventArgs */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        uint32 interruptPipeIndex = SuperMutt::Pipe::InterruptInPipeIndex;

        TypedEventHandler<UsbInterruptInPipe^, UsbInterruptInEventArgs^>^ interruptEventHandler =
            ref new TypedEventHandler<UsbInterruptInPipe^, UsbInterruptInEventArgs^>(
                    this, 
                    &InterruptPipes::OnGeneralInterruptEvent);

        RegisterForInterruptEvent(interruptPipeIndex, interruptEventHandler);

        UpdateRegisterEventButton();
        ClearSwitchStateTable();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void InterruptPipes::UnregisterInterruptEvent_Click(Object^ /* sender */, RoutedEventArgs^ /* eventArgs */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        UnregisterFromInterruptEvent();

        UpdateRegisterEventButton();
        ClearSwitchStateTable();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// This scenario is only available on the SuperMutt device because the OSRFX2 doesn't have Out Interrupt Endpoints.
/// 
/// This method will determine the right InterruptOutPipe to use and will call another method to do the actual writing.
/// <summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
void InterruptPipes::WriteSuperMuttInterruptOut_Click(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* eventArgs */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected
        && (Utilities::IsSuperMuttDevice(EventHandlerForDevice::Current->Device)))
    {
        auto interruptOutPipeIndex = SuperMutt::Pipe::InterruptOutPipeIndex;
        auto interruptOutPipes = EventHandlerForDevice::Current->Device->DefaultInterface->InterruptOutPipes;

        if (interruptOutPipeIndex < interruptOutPipes->Size)
        {
            // We will write the maximum number of bytes we can per interrupt
            auto bytesToWrite = interruptOutPipes->GetAt(interruptOutPipeIndex)->EndpointDescriptor->MaxPacketSize;

            WriteToInterruptOut(interruptOutPipeIndex, bytesToWrite);
        }
        else
        {
            MainPage::Current->NotifyUser("Pipe index provided (" + interruptOutPipeIndex.ToString() + ") is out of range", NotifyType::ErrorMessage);
        }
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// Will write garbage data to the specified output pipe
/// </summary>
/// <param name="pipeIndex">Index of pipe in the list of Device->DefaultInterface->InterruptOutPipes</param>
/// <param name="bytesToWrite">Bytes of garbage data to write</param>
void InterruptPipes::WriteToInterruptOut(uint32 pipeIndex, uint32 bytesToWrite)
{
    auto interruptOutPipes = EventHandlerForDevice::Current->Device->DefaultInterface->InterruptOutPipes;

    if (pipeIndex < interruptOutPipes->Size)
    {
        // Create an array, all default initialized to 0, and write it to the buffer
        // The data inside the buffer will be garbage
        auto arrayBuffer = ref new Array<uint8>(bytesToWrite);

        auto pipe = interruptOutPipes->GetAt(pipeIndex);

        auto pipeWriter = ref new DataWriter(pipe->OutputStream);
        pipeWriter->WriteBytes(arrayBuffer);

        // This is where the data is flushed out to the device.
        create_task(pipeWriter->StoreAsync()).then([this](task<uint32> bytesWrittenTask)
        {
            // An exception may be thrown if writing to the device fails
            auto bytesWritten = bytesWrittenTask.get();

            totalNumberBytesWritten += bytesWritten;

            MainPage::Current->NotifyUser("Total Bytes Written: " + totalNumberBytesWritten.ToString(), NotifyType::StatusMessage);
        });
    }
    else
    {
        MainPage::Current->NotifyUser("Pipe index provided (" + pipeIndex.ToString() + ") is out of range", NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Register for the interrupt that is triggered when the device sends an interrupt to us (e.g. OSRFX2's device's switch state changes
/// or SuperMUTT's software generated interrupts).
/// The DefaultInterface on the the device is the first interface on the device. We navigate to
/// the InterruptInPipes because that collection contains all the interrupt in pipes for the
/// selected interface setting.
///
/// Each pipe has a property that links to an EndpointDescriptor. This descriptor can be used to find information about
/// the pipe (e.g. type, id, etc...). The EndpointDescriptor trys to mirror the EndpointDescriptor that is defined in the Usb Spec.
///
/// The function also saves the event token so that we can unregister from the even later on.
/// </summary>
/// <param name="pipeIndex">The index of the pipe found in UsbInterface::InterruptInPipes. It is not the endpoint number</param>
/// <param name="eventHandler">Event handler that will be called when the event is raised</param>
void InterruptPipes::RegisterForInterruptEvent(
    uint32 pipeIndex, 
    TypedEventHandler<UsbInterruptInPipe^, UsbInterruptInEventArgs^>^ eventHandler)
{
    auto interruptInPipes = EventHandlerForDevice::Current->Device->DefaultInterface->InterruptInPipes;

    if (!registeredInterrupt && (pipeIndex < interruptInPipes->Size))
    {
        auto interruptInPipe = interruptInPipes->GetAt(pipeIndex);
        
        registeredInterruptPipeIndex = pipeIndex;
        registeredInterrupt = true;

        // Save the token so we can unregister from the event later
        interruptEventRegistrationToken = interruptInPipe->DataReceived += eventHandler;

        numInterruptsReceived = 0;
        totalNumberBytesReceived = 0;
    }
}

/// <summary>
/// Unregisters from the interrupt event that was registered for in the RegisterForInterruptEvent();
/// </summary>
void InterruptPipes::UnregisterFromInterruptEvent(void)
{
    if (registeredInterrupt)
    {
        // Search for the correct pipe that we know we used to register events
        auto interruptInPipe = EventHandlerForDevice::Current->Device->DefaultInterface->InterruptInPipes->GetAt(registeredInterruptPipeIndex);

        interruptInPipe->DataReceived -= interruptEventRegistrationToken;
        interruptEventRegistrationToken.Value = 0;

        registeredInterrupt = false;
    }
}

/// <summary>
/// This callback only increments the total number of interrupts received and prints it
///
/// This method is called whenever the device sends an interrupt on the pipe we registered this callback on.
/// </summary>
/// <param name="sender">The interrupt pipe that raised the event (the one that received the interrupt)</param>
/// <param name="eventArgs">Contains the data, in an IBuffer, that was received through the interrupts</param> 
void InterruptPipes::OnGeneralInterruptEvent(UsbInterruptInPipe^ /* sender */, UsbInterruptInEventArgs^  eventArgs )
{
    // If we navigated away from this page, we don't need to process this event
    // This also prevents output from spilling into another page
    if (!navigatedAway)
    {
        numInterruptsReceived++;

        // The data from the interrupt
        IBuffer^ buffer = eventArgs->InterruptData;

        totalNumberBytesReceived += buffer->Length;

        // Create a DispatchedHandler for the because we are interracting with the UI directly and the
        // thread that this function is running on may not be the UI thread; if a non-UI thread modifies
        // the UI, an exception is thrown
        MainPage::Current->Dispatcher->RunAsync(
            CoreDispatcherPriority::Normal,
            ref new DispatchedHandler([this, buffer]()
            {
                // If we navigated away from this page, do not print anything. The dispatch may be handled after
                // we move to a different page.
                if (!navigatedAway)
                {
                    MainPage::Current->NotifyUser(
                        "Total number of interrupt events received: " + numInterruptsReceived.ToString()
                        + "\nTotal number of bytes received: " + totalNumberBytesReceived.ToString(),
                        NotifyType::StatusMessage);
                }
            }));
    }
}

/// <summary>
/// This method is called whenever the device sends an interrupt on the pipe we registered this callback on.
///
/// We will read a byte from the buffer that the device sent to us and then look at each bit to determine state of
/// each switch. AFter determining the state of each switch, we will print out a table with each row representing a 
/// switch and its state.
/// </summary>
/// <param name="sender">The interrupt pipe that raised the event (the one that received the interrupt)</param>
/// <param name="eventArgs">Contains the data, in an IBuffer, that was received through the interrupts</param> 
void InterruptPipes::OnOsrFx2SwitchStateChangeEvent(UsbInterruptInPipe^ /* sender */, UsbInterruptInEventArgs^ eventArgs)
{
    // If we navigated away from this page, we don't need to process this event
    // This also prevents output from spilling into another page
    if (!navigatedAway)
    {
        numInterruptsReceived++;

        // The OSRFX2 gives us 1 byte, each bit representing the state of a switch
        const uint8 numberOfSwitches = 8;   

        auto switchStateArray = ref new Array<bool>(numberOfSwitches);  

        IBuffer^ buffer = eventArgs->InterruptData;

        if (buffer->Length > 0)
        {
            totalNumberBytesReceived += buffer->Length;

            DataReader^ reader = DataReader::FromBuffer(buffer);

            byte switchStates = reader->ReadByte();

            // Loop through each bit of what the device sent us and determine the state of each switch
            for (int switchIndex = 0; switchIndex < numberOfSwitches; switchIndex++)
            {
                int result = (1 << switchIndex) & switchStates;

                if (result != 0)
                {
                    switchStateArray[switchIndex] = true;
                }
                else
                {
                    switchStateArray[switchIndex] = false;
                }
            }

            // Create a DispatchedHandler for the because we are printing the table to the UI directly and the
            // thread that this function is running on may not be the UI thread; if a non-UI thread modifies
            // the UI, an exception is thrown
            MainPage::Current->Dispatcher->RunAsync(
                CoreDispatcherPriority::Normal,
                ref new DispatchedHandler([this, switchStateArray]()
            {
                // If we navigated away from this page, do not print anything. The dispatch may be handled after
                // we move to a different page.
                if (!navigatedAway)
                {
                    MainPage::Current->NotifyUser(
                        "Total number of interrupt events received: " + numInterruptsReceived.ToString()
                        + "\nTotal number of bytes received: " + totalNumberBytesReceived.ToString(),
                        NotifyType::StatusMessage);

                    // Print the switch state table
                    UpdateSwitchStateTable(switchStateArray);
                }
            }));
        }
        else
        {
            // Create a DispatchedHandler for the because we are printing the table to the UI directly and the
            // thread that this function is running on may not be the UI thread; if a non-UI thread modifies
            // the UI, an exception is thrown
            MainPage::Current->Dispatcher->RunAsync(
                CoreDispatcherPriority::Normal,
                ref new DispatchedHandler([this]()
            {
                // If we navigated away from this page, do not print anything. The dispatch may be handled after
                // we move to a different page.
                if (!navigatedAway)
                {
                    MainPage::Current->NotifyUser("Received 0 bytes from interrupt", NotifyType::ErrorMessage);
                }
            }));
        }
    }
}

void InterruptPipes::ClearSwitchStateTable(void)
{
    SwitchStates->Inlines->Clear();
    previousSwitchStates = nullptr;
}

/// <summary>
/// Prints a table in the UI representing the current state of each of the switches on the OSRFX2 board.
/// The bolded states/switches are the switches that have their states changed.
///
/// Note that the OSRFX2 board has the switch states reversed (1 is off and 0 is on)
/// </summary>
/// <param name="states">Switch states where 0 is off and 1 is on</param>
void InterruptPipes::UpdateSwitchStateTable(Platform::Array<bool>^ states)
{
    CreateBooleanChartInTable(
        SwitchStates->Inlines,  // The table will be written directly to the UI's textbox
        states,
        previousSwitchStates,
        "off",
        "on");

    previousSwitchStates = states;
}

void InterruptPipes::UpdateRegisterEventButton(void)
{
    ButtonRegisterOsrFx2InterruptEvent->IsEnabled = !registeredInterrupt;
    ButtonRegisterSuperMuttInterruptEvent->IsEnabled = !registeredInterrupt;
    ButtonUnregisterInterruptEvent->IsEnabled = registeredInterrupt;
}

/// <summary>
/// Clears and populates the provided table with rows that have the following syntax:
/// [row #]    [state of new value]
///
/// If the state of a value has changed, the value will be bolded.
/// </summary>
/// <param name="table">Table will be cleared and new rows will populate this table.</param>
/// <param name="newValues">The new switch states</param>
/// <param name="oldValues">The switch states that the new ones are being compared to</param>
/// <param name="trueValue">Text if the new value is TRUE</param>
/// <param name="falseValue">Text if the new vlaue is FALSE</param>
bool InterruptPipes::CreateBooleanChartInTable(
    InlineCollection^ table,
    const Platform::Array<bool>^ newValues,
    const Platform::Array<bool>^ oldValues,
    String^ trueValue,
    String^ falseValue)
{
    bool isBooleanChartCreated = false;

    // Make sure there are at least newValues to check or that there are the same number of old values as new values
    if ((newValues != nullptr) && ((oldValues == nullptr) || (newValues->Length == oldValues->Length)))
    {
        table->Clear();

        // Each new value has it's own row
        for (int i = 0; i < (int) newValues->Length; i += 1)
        {
            auto line = ref new Span();

            // Row #
            auto block = ref new Run();
            block->Text = (i + 1).ToString();
            line->Inlines->Append(block);

            // Space between row # and the value (simulates a column)
            block = ref new Run();
            block->Text = "    ";
            line->Inlines->Append(block);

            // Print the textual form of TRUE/FALSE values
            block = ref new Run();
            block->Text = newValues[i] ? trueValue : falseValue;

            // Bold values that have changed
            if ((oldValues != nullptr) && (oldValues[i] != newValues[i]))
            {
                auto bold = ref new Bold();
                bold->Inlines->Append(block);
                line->Inlines->Append(bold);
            }
            else
            {
                line->Inlines->Append(block);
            }

            line->Inlines->Append(ref new LineBreak());

            table->Append(line);
        }

        isBooleanChartCreated = true;
    }

    return isBooleanChartCreated;
}
