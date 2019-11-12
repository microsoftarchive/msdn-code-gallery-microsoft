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
// WiFiDirectDeviceScenario.xaml.cpp
// Implementation of the WiFiDirectDeviceScenario class
//



#include "pch.h"
#include "WiFiDirectDevice.xaml.h"

using namespace WiFiDirectDeviceCPP;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::WiFiDirect;
using namespace Windows::Networking;
using namespace concurrency;

WiFiDirectDeviceScenario::WiFiDirectDeviceScenario()
{
    InitializeComponent();
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such 
    // as NotifyUser()
    rootPage = MainPage::Current;

    GetDevicesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;

}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter 
/// property is typically used to configure the page.</param>
void WiFiDirectDeviceScenario::OnNavigatedTo(NavigationEventArgs^ e)
{    
    rootPage->ClearLog(OutputText);    
}

void WiFiDirectDeviceScenario::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
}

void WiFiDirectDeviceScenario::GetDevices(Object^ sender, RoutedEventArgs^ e) 
{
	// Find all discoverable peers with compatible roles
    rootPage->NotifyUser("Enumerating WiFiDirect devices...", NotifyType::StatusMessage);

	// Get the device selector for querying WiFiDirect devices
	String^ deviceSelector = WiFiDirectDevice::GetDeviceSelector();

	create_task(DeviceInformation::FindAllAsync(deviceSelector)).then([this] (task<DeviceInformationCollection ^> resultTask)
    {
        try
        {
			// Clear the list containing the previous discovery results
            FoundDevicesList->Items->Clear();   

            devInfoCollection = resultTask.get();
            if (devInfoCollection && devInfoCollection->Size > 0)
            {
                for (unsigned int i = 0; i < devInfoCollection->Size; i++)
                {                    
                    ComboBoxItem^ item = ref new ComboBoxItem();
                    String^ Name = devInfoCollection->GetAt(i)->Name;
                    item->Content = Name;
                    FoundDevicesList->Items->Append(item);
                }
                FoundDevicesList->SelectedIndex = 0;
                ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
                FoundDevicesList->Visibility = Windows::UI::Xaml::Visibility::Visible;

				rootPage->NotifyUser("Enumerating WiFiDirect devices completed successfully.", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No WiFiDirect devices found.", NotifyType::StatusMessage);
            }
        }
        catch (Exception^ e)
        {
            rootPage->NotifyUser("Exception occurred while finding peer: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void WiFiDirectDeviceScenario::Disconnect(Object^ sender, RoutedEventArgs^ e) 
{
	rootPage->NotifyUser("WiFiDirect device disconnected.", NotifyType::ErrorMessage);

	PCIpAddress->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    DeviceIpAddress->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    DisconnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    FoundDevicesList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    GetDevicesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;

	delete wfdDevice;
}


void WiFiDirectDeviceScenario::Connect(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->NotifyUser("", NotifyType::ErrorMessage);          
    DeviceInformation^ chosenDevInfo;    

    // If nothing is selected, return
    if (FoundDevicesList->SelectedIndex == -1)
    {
        rootPage->NotifyUser("Please select a device.", NotifyType::StatusMessage);
        return;
    }
    else
    {
        chosenDevInfo = devInfoCollection->GetAt(FoundDevicesList->SelectedIndex);
    }

    rootPage->NotifyUser("Connecting to " + chosenDevInfo->Name + "....", NotifyType::StatusMessage);

	create_task(WiFiDirectDevice::FromIdAsync(chosenDevInfo->Id)).then([this, chosenDevInfo] (task<WiFiDirectDevice ^> resultTask)
    {
        try
        {
            wfdDevice = resultTask.get();
            if (wfdDevice == nullptr)
            {
                rootPage->NotifyUser("Connection to " + chosenDevInfo->Name + " failed.", NotifyType::StatusMessage);
                return;
            }

            // Register for Connection status change notification
            disconnectToken = wfdDevice->ConnectionStatusChanged += ref new TypedEventHandler<WiFiDirectDevice^, Object^>(this,
            &WiFiDirectDeviceScenario::DisconnectNotification, CallbackContext::Same);

            // Get the EndpointPair collection
            IVectorView<EndpointPair^>^ endpointPairCollection =  wfdDevice->GetConnectionEndpointPairs();
            if (endpointPairCollection->Size > 0)
            {
                EndpointPair^ endpointPair = endpointPairCollection->GetAt(0);

                PCIpAddress->Text = "PC's IP Address: " + endpointPair->LocalHostName->ToString();
                DeviceIpAddress->Text =  "Device's IP Address: " + endpointPair->RemoteHostName->ToString();

                PCIpAddress->Visibility = Windows::UI::Xaml::Visibility::Visible;
                DeviceIpAddress->Visibility = Windows::UI::Xaml::Visibility::Visible;
                DisconnectButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
                ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                FoundDevicesList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                GetDevicesButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

                rootPage->NotifyUser("Connection succeeded", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("Connection to " + chosenDevInfo->Name + " failed.", NotifyType::StatusMessage);
                return;
            }

            
        }
        catch(Exception ^e)
        {
            rootPage->NotifyUser("Connection to " + chosenDevInfo->Name + " failed: " + e->Message, NotifyType::ErrorMessage);			
        }
    });
}

void WiFiDirectDeviceScenario::DisconnectNotification(WiFiDirectDevice^ sender, Object^ arg)
{
	Dispatcher->RunAsync(CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
    {
        rootPage->NotifyUser("WiFiDirect device disconnected", NotifyType::ErrorMessage);

        GetDevicesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;

        PCIpAddress->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        DeviceIpAddress->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        DisconnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

        // Clear the FoundDevicesList
        FoundDevicesList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        FoundDevicesList->Items->Clear();
    }));

    devInfoCollection = nullptr;
}
