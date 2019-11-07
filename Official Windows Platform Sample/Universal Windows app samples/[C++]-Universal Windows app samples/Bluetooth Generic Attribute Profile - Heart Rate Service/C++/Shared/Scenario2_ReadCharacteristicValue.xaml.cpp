//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_ReadCharacteristicValue.xaml.cpp
// Implementation of the Scenario2_ReadCharacteristicValue class
//

#include "pch.h"
#include "Scenario2_ReadCharacteristicValue.xaml.h"
#include "MainPage.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

using namespace Windows::Devices::Bluetooth::GenericAttributeProfile;

using namespace SDKSample;
using namespace SDKSample::BluetoothGattHeartRate;

Scenario2_ReadCharacteristicValue::Scenario2_ReadCharacteristicValue()
{
    InitializeComponent();
}

void Scenario2_ReadCharacteristicValue::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ /* e */)
{
    if (HeartRateService::Instance->IsServiceInitialized)
    {
        auto bodySensorLocationCharacteristics =
            HeartRateService::Instance->Service->GetCharacteristics(GattCharacteristicUuids::BodySensorLocation);

        if (bodySensorLocationCharacteristics->Size > 0)
        {
            ReadCharacteristicValueButton->IsEnabled = true;
        }
        else
        {
            MainPage::Current->NotifyUser("Your device does not support the Body Sensor Location characteristic.",
                NotifyType::StatusMessage);
        }
    }
    else
    {
        MainPage::Current->NotifyUser("The Heart Rate Service is not initialized, please go to Scenario 1 to initialize " +
            "the service before writing a Characteristic Value", NotifyType::StatusMessage);
    }
}

void Scenario2_ReadCharacteristicValue::ReadValueButton_Click(
    Platform::Object^ sender, 
    Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ReadCharacteristicValueButton->IsEnabled = false;

    auto bodySensorLocationCharacteristic =
        HeartRateService::Instance->Service->GetCharacteristics(GattCharacteristicUuids::BodySensorLocation)->GetAt(0);

    // Read the characteristic value
    create_task(bodySensorLocationCharacteristic->ReadValueAsync())
        .then([this](GattReadResult^ readResult)
    {
        if (readResult->Status == GattCommunicationStatus::Success)
        {
            Array<unsigned char>^ bodySensorLocationData =
                ref new Array<unsigned char>(readResult->Value->Length);

            DataReader::FromBuffer(readResult->Value)->ReadBytes(bodySensorLocationData);

            String^ bodySensorLocation = HeartRateService::Instance->ProcessBodySensorLocationData(
                bodySensorLocationData);

            if (bodySensorLocation->Length() > 0)
            {
                OutputTextBlock->Text = "The Body Sensor Location of your device is : " + bodySensorLocation;
            }
            else
            {
                OutputTextBlock->Text = "The Body Sensor Location is not recognized by this application";
            }
        }
        else
        {
            MainPage::Current->NotifyUser("Your device is unreachable, the device is either out of range, " +
                "or is running low on battery, please make sure your device is working and try again.",
                NotifyType::StatusMessage);
        }
    }).then([this](task<void> finalTask)
    {
        try
        {
            finalTask.get();
        }
        catch (COMException^ e)
        {
            MainPage::Current->NotifyUser("Error: " + e->Message, NotifyType::ErrorMessage);
        };
        Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]() {
            ReadCharacteristicValueButton->IsEnabled = true;
        }));
    });    
}
