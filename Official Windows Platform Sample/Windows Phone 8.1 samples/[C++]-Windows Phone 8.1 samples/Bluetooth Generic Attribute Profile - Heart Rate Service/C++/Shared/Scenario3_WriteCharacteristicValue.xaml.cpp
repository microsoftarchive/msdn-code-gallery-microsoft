//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_WriteCharacteristicValue.xaml.cpp
// Implementation of the Scenario3_WriteCharacteristicValue class
//

#include "pch.h"
#include "Scenario3_WriteCharacteristicValue.xaml.h"
#include "MainPage.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Bluetooth::GenericAttributeProfile;

using namespace SDKSample;
using namespace SDKSample::BluetoothGattHeartRate;

Scenario3_WriteCharacteristicValue::Scenario3_WriteCharacteristicValue()
{
    InitializeComponent();
}

void Scenario3_WriteCharacteristicValue::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ /* e */)
{
    if (HeartRateService::Instance->IsServiceInitialized)
    {
        valueChangeCompletedRegistrationToken = (HeartRateService::Instance->ValueChangeCompleted += 
            ref new SDKSample::BluetoothGattHeartRate::ValueChangeCompletedHandler(
                this, &SDKSample::BluetoothGattHeartRate::Scenario3_WriteCharacteristicValue::Instance_ValueChangeCompleted));


        auto heartRateControlPointCharacteristics =
            HeartRateService::Instance->Service->GetCharacteristics(GattCharacteristicUuids::HeartRateControlPoint);
        if (heartRateControlPointCharacteristics->Size > 0)
        {
            WriteCharacteristicValueButton->IsEnabled = true;
        }
        else
        {
            MainPage::Current->NotifyUser("Your device does not support the Expended Energy characteristic.",
                NotifyType::StatusMessage);
        }
    }
    else
    {
        MainPage::Current->NotifyUser("The Heart Rate Service is not initialized, please go to Scenario 1 to initialize " +
            "the service before writing a Characteristic Value", NotifyType::StatusMessage);
    }
}

void Scenario3_WriteCharacteristicValue::OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ /* e */)
{
    HeartRateService::Instance->ValueChangeCompleted -= valueChangeCompletedRegistrationToken;
}

void Scenario3_WriteCharacteristicValue::Instance_ValueChangeCompleted(HeartRateMeasurement^ heartRateMeasurementValue)
{
    //Serialize UI update to the main UI Thread
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, heartRateMeasurementValue] () 
    {
        if (heartRateMeasurementValue->HasExpendedEnergy)
        {
            std::wstringstream wss;
            wss << L"Expended Energy: " << heartRateMeasurementValue->ExpendedEnergy << L" kJ";

            ExpendedEnergyTextBlock->Text = ref new String(wss.str().c_str());
        }
        else
        {
            ExpendedEnergyTextBlock->Text = "The optional Expended Energy value was not received when reading data " +
                "from your device";
        }
    }));
}

void Scenario3_WriteCharacteristicValue::WriteCharacteristicValue_Click(Object^ sender, RoutedEventArgs^ e)
{
    WriteCharacteristicValueButton->IsEnabled = false;

    auto heartRateControlPointCharacteristic =
        HeartRateService::Instance->Service->GetCharacteristics(GattCharacteristicUuids::HeartRateControlPoint)->GetAt(0);

    auto writer = ref new Windows::Storage::Streams::DataWriter();
    writer->WriteByte(1);

    create_task(heartRateControlPointCharacteristic->WriteValueAsync(writer->DetachBuffer()))
        .then([this](GattCommunicationStatus status)
    {
        if (status == GattCommunicationStatus::Success)
        {
            MainPage::Current->NotifyUser("Expended Energy successfully reset.", NotifyType::StatusMessage);
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
        }
        Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]() {
            WriteCharacteristicValueButton->IsEnabled = true;
        }));
    });
}
