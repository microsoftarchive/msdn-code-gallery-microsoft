//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_DeviceEvents.xaml.cpp
// Implementation of the Scenario1_DeviceEvents class
//

#include "pch.h"
#include <algorithm>
#include "Scenario1_DeviceEvents.xaml.h"
#include "MainPage.xaml.h"

using namespace concurrency;
using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Enumeration::Pnp;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Bluetooth::GenericAttributeProfile;

using namespace SDKSample;
using namespace SDKSample::BluetoothGattHeartRate;

Scenario1_DeviceEvents::Scenario1_DeviceEvents()
{
    InitializeComponent();
}

void Scenario1_DeviceEvents::OnNavigatedTo(NavigationEventArgs^ /* e */)
{
    if (HeartRateService::Instance->IsServiceInitialized)
    {
        for_each(begin(HeartRateService::Instance->DataPoints), end(HeartRateService::Instance->DataPoints),
            [this](HeartRateMeasurement^ value)
        {
            outputListBox->Items->Append(value->GetDescription());
        });
        outputGrid->Visibility = Windows::UI::Xaml::Visibility::Visible;
#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
        outputDataChart->Visibility = Windows::UI::Xaml::Visibility::Visible;
#endif
        RunButton->IsEnabled = false;
    }
    valueChangeCompletedRegistrationToken = (HeartRateService::Instance->ValueChangeCompleted += 
        ref new ValueChangeCompletedHandler(this, &Scenario1_DeviceEvents::Instance_ValueChangeCompleted));
}

void Scenario1_DeviceEvents::OnNavigatedFrom(NavigationEventArgs^ /* e */)
{
    HeartRateService::Instance->ValueChangeCompleted -= valueChangeCompletedRegistrationToken;
}

void SDKSample::BluetoothGattHeartRate::Scenario1_DeviceEvents::OutputDataChart_SizeChanged(
    Platform::Object^ /*sender*/, Windows::UI::Xaml::SizeChangedEventArgs^ /*e*/)
{
#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
    outputDataChart->PlotChart(HeartRateService::Instance->DataPoints);
#endif
}

void Scenario1_DeviceEvents::Instance_ValueChangeCompleted(HeartRateMeasurement^ heartRateMeasurementValue)
{
    //Serialize UI update to the main UI Thread
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, heartRateMeasurementValue] ()
    {
        std::wstringstream wss;
        wss << L"Latest received heart rate measurement: " << heartRateMeasurementValue->HeartRateValue;
        statusTextBlock->Text = ref new String(wss.str().c_str());

#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
        outputDataChart->PlotChart(HeartRateService::Instance->DataPoints);
#endif

        outputListBox->Items->InsertAt(0, heartRateMeasurementValue->GetDescription());
    }));
}

void Scenario1_DeviceEvents::RunButton_Click(Object ^ /* sender */, RoutedEventArgs ^ /* e */)
{
    RunButton->IsEnabled = false;

    Vector<String^>^ additionalProperties = ref new Vector<String^>;
    additionalProperties->Append("System.Devices.ContainerId");

    create_task(DeviceInformation::FindAllAsync(
        GattDeviceService::GetDeviceSelectorFromUuid(GattServiceUuids::HeartRate), additionalProperties))
        .then([this](Windows::Devices::Enumeration::DeviceInformationCollection^ devices)
    {
        this->devices = devices;

        if (devices->Size > 0)
        {
            Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, devices]()
            {
                DevicesListBox->Items->Clear();

                auto serviceNames = ref new Vector<String^>();

                for_each(begin(devices), end(devices), [=](DeviceInformation^ device) 
                {
                    serviceNames->Append(device->Name);
                });

                cvs->Source = serviceNames;
                DevicesListBox->Visibility = Windows::UI::Xaml::Visibility::Visible;
            }));
        }
        else
        {
            MainPage::Current->NotifyUser(L"Could not find any Heart Rate devices. Please make sure your " +
                "device is paired and powered on!",
                NotifyType::StatusMessage);
        }

        Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]() 
        {
            RunButton->IsEnabled = true;
        }));
    }).then([](task<void> finalTask)
    {
        try
        {
            // Capture any errors and exceptions that occured during device discovery
            finalTask.get();
        }
        catch (COMException^ e)
        {
            MainPage::Current->NotifyUser("Error: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void Scenario1_DeviceEvents::DevicesListBox_Tapped(Object ^ /* sender */, TappedRoutedEventArgs ^ /* e */)
{
    RunButton->IsEnabled = false;
    DevicesListBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    auto device = devices->GetAt(DevicesListBox->SelectedIndex);

    statusTextBlock->Text = "Initializing device...";
    HeartRateService::Instance->DeviceConnectionUpdated += ref new DeviceConnectionUpdatedHandler(this,
        &Scenario1_DeviceEvents::OnDeviceConnectionUpdated);
    create_task(HeartRateService::Instance->InitializeHeartRateServicesAsync(device))
        .then([this, device]()
    {
        return create_task(Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            outputGrid->Visibility = Windows::UI::Xaml::Visibility::Visible;

#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
            outputDataChart->Visibility = Windows::UI::Xaml::Visibility::Visible;
#endif
        }))).then([this, device](){

            String^ deviceContainerId = device->Properties->Lookup("System.Devices.ContainerId")->ToString();

            Vector<String^>^ connectedProperty = ref new Vector<String^>();
            connectedProperty->Append("System.Devices.Connected");

            // Check if the device is initially connected, and display the appropriate message to the user
            return create_task(PnpObject::CreateFromIdAsync(PnpObjectType::DeviceContainer, deviceContainerId, 
                connectedProperty)).then([this](PnpObject^ deviceObject)
            {
                auto connectedProperty = reinterpret_cast<IPropertyValue^>(
                    deviceObject->Properties->Lookup("System.Devices.Connected"));
                auto isConnected = connectedProperty->GetBoolean();
                OnDeviceConnectionUpdated(isConnected);
            });
        });
    }).then([](task<void> finalTask)
    {
        try
        {
            // Capture any errors and exceptions that occured
            finalTask.get();
        }
        catch (COMException^ e)
        {
            MainPage::Current->NotifyUser("Error: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void Scenario1_DeviceEvents::OnDeviceConnectionUpdated(boolean isConnected)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, isConnected]()
    {
        if (isConnected)
        {
            statusTextBlock->Text = "Waiting for device to send data...";
        }
        else
        {
            statusTextBlock->Text = "Waiting for device to connect...";
        }
    }));
}
