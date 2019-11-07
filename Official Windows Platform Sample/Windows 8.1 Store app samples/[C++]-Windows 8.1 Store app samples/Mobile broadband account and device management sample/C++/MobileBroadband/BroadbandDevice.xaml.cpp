//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// BroadbandDevice.xaml.cpp
// Implementation of the BroadbandDevice class
//

#include "pch.h"
#include "BroadbandDevice.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::Networking::Connectivity;
using namespace Windows::Devices::Sms;


BroadbandDevice::BroadbandDevice():
    deviceSelected(0)
{
    InitializeComponent();

    networkAccountWatcher = ref new MobileBroadbandAccountWatcher();
}

//
// Initialize variables and controls for the scenario
// This method is called just before the scenario page is displayed
//
void BroadbandDevice::OnNavigatedTo(NavigationEventArgs^ e)
{
    //
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    //
    rootPage = MainPage::Current;

    PrepareScenario();
}

void BroadbandDevice::UpdateData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    GetCurrentDeviceInfo(deviceAccountIds->GetAt(deviceSelected));

    //
    // Increment device count until reach number max number of devices and then start over
    //
    deviceSelected = (deviceSelected + 1) % deviceAccountIds->Size;

    //
    // Update Button with next device
    //
    UpdateData->Content = "Get Information for Device #" + (deviceSelected + 1);
}

Platform::String^ BroadbandDevice::NetErrorToString(uint32_t netError)
{
    return netError == 0 ? "none" : netError.ToString();
}

void BroadbandDevice::PrepareScenario()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);

    try
    {
        deviceSelected = 0;
        deviceAccountIds = MobileBroadbandAccount::AvailableNetworkAccountIds;

        if (deviceAccountIds->Size != 0)
        {
            rootPage->NotifyUser("Mobile Broadband Device(s) have been installed that grant access to this application", NotifyType::StatusMessage);
            NumDevices->Text = "There are " + deviceAccountIds->Size + " account(s) installed.";

            UpdateData->Content = "Get Information for Device #1";
            UpdateData->IsEnabled = true;
        }
        else
        {
            UpdateData->Content = "No available accounts detected";
            UpdateData->IsEnabled = false;
        }
    }
    catch (Platform::COMException^ ex)
    {
        rootPage->NotifyUser("Error:" + ex->ToString(), NotifyType::ErrorMessage);
    }
}

// Function returns a string of DataClasses values
// Bitwise shift operation is used to convert DataClasses enum to string
// due to ToString() method currently not supporting multiple flag values
Platform::String^ DataClassToString(DataClasses dataClasses)
{
    Platform::String^ dataClassesString = "";

    // If no DataClass is present, return "None"
    if (dataClasses == DataClasses::None)
        return DataClasses::None.ToString();

    // Enum variable is used to hold a value to compare to
    auto currentDataClass = static_cast<DataClasses>(1U);

    // Compare and if match add value to string to be displayed.
    // currentDataClass value will be DataClasses::None on bitshift overflow
    while (currentDataClass != DataClasses::None)
    {
        if (static_cast<unsigned int> (dataClasses) & static_cast<unsigned int> (currentDataClass))
        {
            if (dataClassesString != "")
            {
                dataClassesString += "/";
            }
            dataClassesString += currentDataClass.ToString();
        }
        // Shift the Enum value one bit left
        currentDataClass = static_cast<DataClasses>(static_cast<unsigned int>(currentDataClass) << 1);
    }
    return dataClassesString;
}

void BroadbandDevice::GetCurrentDeviceInfo(Platform::String^ accountId)
{
    try
    {
        auto mobileBroadbandAccount = MobileBroadbandAccount::CreateFromNetworkAccountId(accountId);

        ProviderName->Text = mobileBroadbandAccount->ServiceProviderName;
        ProviderGuid->Text = mobileBroadbandAccount->ServiceProviderGuid.ToString();
        NetworkAccountId->Text = mobileBroadbandAccount->NetworkAccountId;

        auto currentNetwork = mobileBroadbandAccount->CurrentNetwork;

        if (currentNetwork != nullptr)
        {
            Platform::String^ accessPointName = currentNetwork->AccessPointName;
            if (accessPointName == nullptr)
            {
                accessPointName = "(not connected)";
            }
            else if (accessPointName->IsEmpty())
            {
                accessPointName = "(not connected)";
            }

            NetRegister->Text = currentNetwork->NetworkRegistrationState.ToString();
            NetRegError->Text = NetErrorToString(currentNetwork->RegistrationNetworkError);
            PacketAttachError->Text = NetErrorToString(currentNetwork->PacketAttachNetworkError);
            ActivateError->Text = NetErrorToString(currentNetwork->ActivationNetworkError);
            AccessPointName->Text = accessPointName;
            NetworkAdapterId->Text = currentNetwork->NetworkAdapter->NetworkAdapterId.ToString();

            auto networkInterfaceType = currentNetwork->NetworkAdapter->NetworkItem->GetNetworkTypes();
            switch(networkInterfaceType)
            {
            case (NetworkTypes::None):
                NetworkType->Text = "None";
                break;
            case (NetworkTypes::Internet):
                NetworkType->Text = "Internet";
                break;
            case (NetworkTypes::PrivateNetwork):
                NetworkType->Text = "Private Network";
                break;
            case (NetworkTypes::Internet | NetworkTypes::PrivateNetwork):
                NetworkType->Text = "Internet, Private Network";
                break;
            }

            RegisteredProviderId->Text = currentNetwork->RegisteredProviderId;
            RegisteredProviderName->Text = currentNetwork->RegisteredProviderName;
            RegisteredDataClass->Text = DataClassToString(currentNetwork->RegisteredDataClass);
        }
        else
        {
            NetRegister->Text = "";
            NetRegError->Text = "";
            PacketAttachError->Text = "";
            ActivateError->Text = "";
            AccessPointName->Text = "";
            NetworkAdapterId->Text = "";
            NetworkType->Text = "";
            RegisteredProviderId->Text = "";
            RegisteredProviderName->Text = "";
            RegisteredDataClass->Text = "";
        }

        auto deviceInformation = mobileBroadbandAccount->CurrentDeviceInformation;

        if (deviceInformation != nullptr)
        {
            Platform::String^ mobileNumber = "";
            if (deviceInformation->TelephoneNumbers->Size > 0)
            {
                mobileNumber = deviceInformation->TelephoneNumbers->GetAt(0);
            }

            DeviceManufacturer->Text = deviceInformation->Manufacturer;
            DeviceModel->Text = deviceInformation->Model;
            Firmware->Text = deviceInformation->FirmwareInformation;
            CellularClasses->Text = deviceInformation->CellularClass.ToString();

            DataClasses->Text = DataClassToString(deviceInformation->DataClasses);

            if ((unsigned int) deviceInformation->DataClasses & (unsigned int) DataClasses::Custom)
            {
                DataClasses->Text += " (custom is " + deviceInformation->CustomDataClass + ")";
            }

            MobileNumber->Text = mobileNumber;
            SimId->Text = deviceInformation->SimIccId;

            DeviceType->Text = deviceInformation->DeviceType.ToString();
            DeviceId->Text = deviceInformation->DeviceId;

            NetworkDeviceStatus->Text = deviceInformation->NetworkDeviceStatus.ToString();

            if (deviceInformation->CellularClass == CellularClass::Gsm)
            {
                MobEquipIdLabel->Text = "IMEI:";
                MobEquipIdValue->Text = deviceInformation->MobileEquipmentId;

                SubIdLabel->Text = "IMSI:";
                SubIdValue->Text = deviceInformation->SubscriberId;
            }
            else if (deviceInformation->CellularClass == CellularClass::Cdma)
            {
                MobEquipIdLabel->Text = "ESN/MEID:";
                MobEquipIdValue->Text = deviceInformation->MobileEquipmentId;

                SubIdLabel->Text = "MIN/IRM:";
                SubIdValue->Text = deviceInformation->SubscriberId;
            }
            else
            {
                MobEquipIdLabel->Text = "";
                MobEquipIdValue->Text = "";
                SubIdLabel->Text = "";
                SubIdValue->Text = "";
            }
        }
        else
        {
            DeviceManufacturer->Text = "";
            DeviceModel->Text = "";
            Firmware->Text = "";
            CellularClasses->Text = "";
            DataClasses->Text = "";
            MobileNumber->Text = "";
            SimId->Text = "";
            DeviceType->Text = "";
            NetworkDeviceStatus->Text = "";
            MobEquipIdLabel->Text = "";
            MobEquipIdValue->Text = "";
            SubIdLabel->Text = "";
            SubIdValue->Text = "";
        }
    }
    catch (Platform::COMException^ ex)
    {
        rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);

        ProviderName->Text = "";
        ProviderGuid->Text = "";
        NetworkAccountId->Text = "";
        NetRegister->Text = "";
        NetRegError->Text = "";
        PacketAttachError->Text = "";
        ActivateError->Text = "";
        AccessPointName->Text = "";
        NetworkAdapterId->Text = "";
        NetworkType->Text = "";
        DeviceManufacturer->Text = "";
        DeviceModel->Text = "";
        Firmware->Text = "";
        CellularClasses->Text = "";
        DataClasses->Text = "";
        MobileNumber->Text = "";
        SimId->Text = "";
        DeviceType->Text = "";
        DeviceId->Text = "";
        NetworkDeviceStatus->Text = "";
        MobEquipIdLabel->Text = "";
        MobEquipIdValue->Text = "";
        SubIdLabel->Text = "";
        SubIdValue->Text = "";
        RegisteredProviderId->Text = "";
        RegisteredProviderName->Text = "";
        RegisteredDataClass->Text = "";
    }
}
