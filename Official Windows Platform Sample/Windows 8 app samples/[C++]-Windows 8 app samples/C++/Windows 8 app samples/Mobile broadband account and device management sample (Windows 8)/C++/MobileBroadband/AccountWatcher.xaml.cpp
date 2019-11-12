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
// AccountWatcher.xaml.cpp
// Implementation of the AccountWatcher class
//

#include "pch.h"
#include "AccountWatcher.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::UI::Core;
using namespace Windows::Foundation;

AccountWatcher::AccountWatcher()
{
    InitializeComponent();
    networkAccountWatcher = ref new MobileBroadbandAccountWatcher();

    //
    // Reference to the main window dispatcher object to the UI
    //
    sampleDispatcher = Window::Current->CoreWindow->Dispatcher;
}

//
// Initialize variables and controls for the scenario
// This method is called just before the scenario page is displayed
//
void AccountWatcher::OnNavigatedTo(NavigationEventArgs^ e)
{
    //
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    //
    rootPage = MainPage::Current;

    PrepareWatcher();
}

void AccountWatcher::StartMonitoring_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if ((networkAccountWatcher->Status == MobileBroadbandAccountWatcherStatus::Started) ||
    (networkAccountWatcher->Status == MobileBroadbandAccountWatcherStatus::EnumerationCompleted))
    {
        rootPage->NotifyUser("Watcher is already started.", NotifyType::StatusMessage);
    }
    else
    {
        try
        {
            networkAccountWatcher->Start();
            rootPage->NotifyUser("Watcher is started successfully.", NotifyType::StatusMessage);
        }
        catch (Platform::COMException^ ex)
        {
            rootPage->NotifyUser("Error: " + ex->ToString(), NotifyType::ErrorMessage);
        }
    }
}

void AccountWatcher::StopMonitoring_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if ((networkAccountWatcher->Status == MobileBroadbandAccountWatcherStatus::Started) ||
    (networkAccountWatcher->Status == MobileBroadbandAccountWatcherStatus::EnumerationCompleted))
    {
        try
        {
            networkAccountWatcher->Stop();
            rootPage->NotifyUser("Watcher is stopped successfully.", NotifyType::StatusMessage);
        }
        catch (Platform::COMException^ ex)
        {
            rootPage->NotifyUser("Error: " + ex->ToString(), NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->NotifyUser("Watcher is already stopped.", NotifyType::StatusMessage);
    }
}

Platform::String^ AccountWatcher::NetErrorToString(uint32_t netError)
{
    return netError == 0 ? "none" : netError.ToString();
}

void AccountWatcher::PrepareWatcher()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);

    networkAccountWatcher->AccountAdded += ref new TypedEventHandler<MobileBroadbandAccountWatcher^, MobileBroadbandAccountEventArgs^> (
        [this] (MobileBroadbandAccountWatcher^ sender, MobileBroadbandAccountEventArgs^ args)
    {
            Platform::String^ message = "[accountadded] ";

            try
            {
                message += args->NetworkAccountId;
                auto account = MobileBroadbandAccount::CreateFromNetworkAccountId(args->NetworkAccountId);
                message += ", service provider name: " + account->ServiceProviderName  + "\n";
            }
            catch (Platform::Exception^ ex)
            {
                message += ex->Message;
            }

            DisplayWatcherOutputFromCallback(message);
    });

    networkAccountWatcher->AccountUpdated += ref new TypedEventHandler<MobileBroadbandAccountWatcher^, MobileBroadbandAccountUpdatedEventArgs^> (
        [this] (MobileBroadbandAccountWatcher^ sender, MobileBroadbandAccountUpdatedEventArgs^ args)
    {
            Platform::String^ message = "[accountupdated] ";
            try
            {
                message += args->NetworkAccountId + ", (network = " + args->HasNetworkChanged + "; deviceinformation = " + args->HasDeviceInformationChanged + ")" + "\n";
                message += DumpPropertyData(args->NetworkAccountId, args->HasNetworkChanged, args->HasDeviceInformationChanged);
            }
            catch (Platform::Exception^ ex)
            {
                message += ex->Message;
            }

            DisplayWatcherOutputFromCallback(message);
    });

    networkAccountWatcher->AccountRemoved += ref new TypedEventHandler<MobileBroadbandAccountWatcher^, MobileBroadbandAccountEventArgs^> (
        [this] (MobileBroadbandAccountWatcher^ sender, MobileBroadbandAccountEventArgs^ args)
    {
            Platform::String^ message = "[accountremoved] ";

            try
            {
                message += args->NetworkAccountId;
            }
            catch (Platform::Exception^ ex)
            {
                message += ex->Message;
            }

            DisplayWatcherOutputFromCallback(message);
    });

    networkAccountWatcher->EnumerationCompleted += ref new TypedEventHandler<MobileBroadbandAccountWatcher^, Platform::Object^> (
        [this] (MobileBroadbandAccountWatcher^ sender, Platform::Object^ args)
    {
            Platform::String^ message = "[enumerationcompleted]";

            DisplayWatcherOutputFromCallback(message);
    });

    networkAccountWatcher->Stopped += ref new TypedEventHandler<MobileBroadbandAccountWatcher^, Platform::Object^> (
        [this] (MobileBroadbandAccountWatcher^ sender, Platform::Object^ args)
    {
            Platform::String^ message = "[stopped] Watcher is stopped successfully";

            DisplayWatcherOutputFromCallback(message);
    });
}

void AccountWatcher::DisplayWatcherOutputFromCallback(Platform::String^ value)
{
    sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler ([this, value] ()
    {
        WatcherOutput->Text += value + "\n";
    }));
}

Platform::String^ AccountWatcher::DumpAccountDeviceInformation(MobileBroadbandDeviceInformation^ deviceInformation)
{
    Platform::String^ tempstring = "NetworkDeviceStatus: ";

    tempstring += deviceInformation->NetworkDeviceStatus.ToString() + "\n";
    tempstring += "MobileEquipmentId: " + deviceInformation->MobileEquipmentId + "\n";
    tempstring += "SubscriberId: " + deviceInformation->SubscriberId + "\n";
    tempstring += "SimIccId: " + deviceInformation->SimIccId + "\n";
    tempstring += "DeviceId: " + deviceInformation->DeviceId + "\n";
    tempstring += "RadioState: " + deviceInformation->CurrentRadioState.ToString() + "\n";

    return tempstring;
}

Platform::String^ AccountWatcher::DumpAccountNetwork(MobileBroadbandNetwork^ network)
{
    Platform::String^ accessPointName = network->AccessPointName;
    if (accessPointName == nullptr)
    {
        accessPointName = "(not connected)";
    }
    else if (accessPointName->IsEmpty())
    {
        accessPointName = "(not connected)";
    }

    Platform::String^ tempstring = "NetworkRegistrationState: " + network->NetworkRegistrationState.ToString() + "\n";
    tempstring += "RegistrationNetworkError: " + NetErrorToString(network->RegistrationNetworkError) + "\n";
    tempstring += "PacketAttachNetworkError: " + NetErrorToString(network->PacketAttachNetworkError) + "\n";
    tempstring += "ActivationNetworkError: " + NetErrorToString(network->ActivationNetworkError) + "\n";
    tempstring += "AccessPointName: " + accessPointName + "\n";

    return tempstring;
}

Platform::String^ AccountWatcher::DumpPropertyData(Platform::String^ networkAccountId, bool hasDeviceInformationChanged, bool hasNetworkChanged)
{
    Platform::String^ message;

    auto account = MobileBroadbandAccount::CreateFromNetworkAccountId(networkAccountId);

    Platform::String^ tempstring1;
    Platform::String^ tempstring2;

    if (hasDeviceInformationChanged)
    {
        tempstring1 = DumpAccountDeviceInformation(account->CurrentDeviceInformation);
    }

    if (hasNetworkChanged)
    {
        tempstring2 = DumpAccountNetwork(account->CurrentNetwork);
    }

    message = tempstring1 + tempstring2;

    return message;
}