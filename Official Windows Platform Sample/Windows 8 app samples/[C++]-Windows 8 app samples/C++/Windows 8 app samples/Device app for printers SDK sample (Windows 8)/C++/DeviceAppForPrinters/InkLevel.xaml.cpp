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
// InkLevel.xaml.cpp
// Implementation of the InkLevel class
//

#include "pch.h"
#include "InkLevel.xaml.h"

using namespace DeviceAppForPrinters;

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Enumeration;

InkLevel::InkLevel()
{
    InitializeComponent();

    _keyPrinterName = ref new Platform::String(L"BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39");
    _keyAsyncUIXML = ref new Platform::String(L"55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685");

    // Replace the string with your own package family name 
    _selfPackageFamilyName = ref new Platform::String(L"Microsoft.SDKSamples.DeviceAppForPrinters.CPP_8wekyb3d8bbwe");
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void InkLevel::OnNavigatedTo(NavigationEventArgs^ e)
{	
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Get the notification data
    Windows::Storage::ApplicationDataContainer^ settings = Windows::Storage::ApplicationData::Current->LocalSettings;
    Platform::String^ printerName = safe_cast<Platform::String^>(settings->Values->Lookup(_keyPrinterName));
    Platform::String^ asyncUIXML = safe_cast<Platform::String^>(settings->Values->Lookup(_keyAsyncUIXML));

    // Display notification data
    rootPage->NotifyUser("Notification updated", NotifyType::StatusMessage);
    ToastOutput->Text = "Printer name from background task triggerDetails: " + printerName + "\n" 
        + "AsyncUI xml from background task triggerDetails: " + asyncUIXML;

    // Clear any tile/badge after user launching the app
    Windows::UI::Notifications::TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
    Windows::UI::Notifications::BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Clear();
}

void DeviceAppForPrinters::InkLevel::GetInkLevel(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ComboBoxItem^ item = safe_cast<ComboBoxItem^>(AssociatedPrinters->SelectedItem);
    if (nullptr != item)
    {
        Platform::String^ printerName = safe_cast<Platform::String^>(item->Content);
        Platform::String^ printerInterfaceId = safe_cast<Platform::String^>(item->DataContext);
        try
        {
            Platform::Object^ context = Windows::Devices::Printers::Extensions::PrintExtensionContext::FromDeviceId(printerInterfaceId);
            rootPage->PrinterExtensionContext = context;
            PrintHelper^ printHelper = rootPage->PrintHelper;
            if (nullptr != printHelper)
            {
                rootPage->NotifyUser("Retrieving ink level", NotifyType::WaitMessage);
                _asyncOperation = printHelper->GetInkLevelAsync();

                // Enable Cancel button and disable InkLevel button
                // We don't want to create multiple asyncOperation
                CancelButton->IsEnabled = true;
                InkLevelButton->IsEnabled = false;

                task<Platform::String^>(_asyncOperation).then(
                    [this](Platform::String^ inkLevel)
                {
                    // Update UI with the result
                    if (nullptr != inkLevel)
                    {
                        BidiOutput->Text = inkLevel + L"\r\n" + BidiOutput->Text;
                        rootPage->NotifyUser("Ink Level Ready", NotifyType::StatusMessage);
                    }
                    else
                    {
                        rootPage->NotifyUser(L"Failed to retrieve ink level", NotifyType::ErrorMessage);
                    }

                    // Disable Cancel button and enable InkLevel button
                    CancelButton->IsEnabled = false;
                    InkLevelButton->IsEnabled = true;

                    _asyncOperation = nullptr;
                });
            }
        }
        catch (Platform::Exception^ exception)
        {
            rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
        }
    }
}

void DeviceAppForPrinters::InkLevel::Cancel(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Cancel the ongoing async operation
    if (nullptr != _asyncOperation)
    {
        _asyncOperation->Cancel();
        _asyncOperation = nullptr;
    }
    
    // Disable Cancel button and enable InkLevel button
    CancelButton->IsEnabled = false;
    InkLevelButton->IsEnabled = true;

    rootPage->NotifyUser(L"Operation cancelled", NotifyType::ErrorMessage);
}

void DeviceAppForPrinters::InkLevel::EnumeratePrinters(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Clean up the combo box and notify user
    rootPage->NotifyUser("Enumerating printers associated with this app", NotifyType::WaitMessage);
    AssociatedPrinters->Items->Clear();

    try
    {
        // Selector string for printers
        String^ selector(L"System.Devices.InterfaceClassGuid:=\"{0ecef634-6ef0-472a-8085-5ad023ecbccd}\"");

        // Look for the container ID and printer name for each device interface
        auto propertiesToRetrieve = ref new Platform::Collections::Vector<String^>();
        propertiesToRetrieve->Append("System.Devices.ContainerId");
        propertiesToRetrieve->Append("System.DeviceInterface.PrinterName");

        // Create an async task to look for all the printer device interfaces
        task<DeviceInformationCollection^> (DeviceInformation::FindAllAsync(selector, propertiesToRetrieve)).then(
            [this](DeviceInformationCollection^ deviceInfoCollection)
        {
            // Enumerate each device interface and show a message when done
            EnumerateInterface(deviceInfoCollection);
        });
    }
    catch (Platform::Exception^ exception)
    {
        rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
    }
}

void DeviceAppForPrinters::InkLevel::EnumerateInterface(Windows::Devices::Enumeration::DeviceInformationCollection^ deviceInfoCollection)
{
    unsigned int count = deviceInfoCollection->Size;
    for (unsigned int i = 0; i < count; i++)    
    {
        DeviceInformation^ deviceInfo = deviceInfoCollection->GetAt(i);
        try
        {
            // Get the device container GUID, printer name and interface ID
            Platform::Guid guidId = safe_cast<Platform::Guid>(deviceInfo->Properties->Lookup("System.Devices.ContainerId"));
            Platform::String^ printerName = safe_cast<Platform::String^>(deviceInfo->Properties->Lookup("System.DeviceInterface.PrinterName"));
            Platform::String^ interfaceId = deviceInfo->Id;

            // Look for the app package family name for this container
            auto propertiesToRetrieve = ref new Platform::Collections::Vector<String^>();
            propertiesToRetrieve->Append("System.Devices.AppPackageFamilyName");

            // Asynchoronously getting the container information of the printer.
            task<Pnp::PnpObject^> (Pnp::PnpObject::CreateFromIdAsync(Pnp::PnpObjectType::DeviceContainer, guidId.ToString(), propertiesToRetrieve)).then(
                [this, interfaceId, printerName, i, count](Pnp::PnpObject^ pnpObj)
            {
                // Check if the app package name for this container is a match
                bool isMatch = EnumerateContainer(pnpObj);
                if (isMatch)
                {
                    // This is a match, add it to the list
                    rootPage->AddMessage(L"[" + printerName + "] " + interfaceId);
                    ComboBoxItem^ item = ref new ComboBoxItem();
                    item->Content = printerName;
                    item->DataContext = interfaceId;
                    AssociatedPrinters->Items->Append(item);
                }
                else
                {
                    // Not a match, skip
                    rootPage->AddMessage(L"[" + printerName + "] not a match");
                }

                // Check if this is the last async operation and update UI accordingly
                if (i == count-1)
                {
                    if (AssociatedPrinters->Items->Size > 0)
                    {
                        AssociatedPrinters->SelectedIndex = 0;
                    }
                    rootPage->NotifyUser("Printers enumerated", NotifyType::StatusMessage);
                }
            });
        }
        catch (Platform::Exception^ exception)
        {
            rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
        }
    }
}

bool DeviceAppForPrinters::InkLevel::EnumerateContainer(Windows::Devices::Enumeration::Pnp::PnpObject^ pnpObject)
{
    bool isMatch = false;
    try
    {
        // Look up the package family name which is a IPropertyValue
        Windows::Foundation::IPropertyValue^ packageFamilyNameObject = safe_cast<Windows::Foundation::IPropertyValue^>(pnpObject->Properties->Lookup("System.Devices.AppPackageFamilyName"));
        if (packageFamilyNameObject != nullptr)
        {
            // Convert property value to a string array
            Platform::Array<Platform::String^>^ packageFamilyNameArray;
            packageFamilyNameObject->GetStringArray(&packageFamilyNameArray);
            if (nullptr != packageFamilyNameArray)
            {
                // Iterate through the string array
                for (unsigned int i=0; i<packageFamilyNameArray->Length; i++)
                {
                    // Compare the package family name with this app
                    Platform::String^ packageFamilyName = packageFamilyNameArray->Data[i];
                    if (packageFamilyName == _selfPackageFamilyName)
                    {
                        isMatch = true;
                        break;
                    }
                }            
            }
        }
    }
    catch (Platform::Exception^ exception)
    {
        rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
    }
    
    return isMatch;
}
