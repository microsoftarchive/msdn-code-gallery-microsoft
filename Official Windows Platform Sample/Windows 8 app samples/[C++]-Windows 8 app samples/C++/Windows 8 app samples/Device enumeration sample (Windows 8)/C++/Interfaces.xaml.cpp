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
// Interfaces.xaml.cpp
// Implementation of the Interfaces class
//

#include "pch.h"
#include "Interfaces.xaml.h"

using namespace SDKSample::DeviceEnumeration;

Interfaces::Interfaces()
{
    InitializeComponent();
}

void Interfaces::InterfaceClasses_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ eventArgs)
{
    if (InterfaceClasses->SelectedItem->Equals(PrinterInterfaceClass))
    {
        InterfaceClassGuid->Text = "{0ECEF634-6EF0-472A-8085-5AD023ECBCCD}";
    }
    else if (InterfaceClasses->SelectedItem->Equals(WebcamInterfaceClass))
    {
        InterfaceClassGuid->Text = "{E5323777-F976-4F5B-9B55-B94699C46E44}";
    }
    else if (InterfaceClasses->SelectedItem->Equals(WpdInterfaceClass))
    {
        InterfaceClassGuid->Text = "{6AC27878-A6FA-4155-BA85-F98F491D4F33}";
    }
}

void Interfaces::EnumerateDeviceInterfaces(Object^ sender, RoutedEventArgs^ eventArgs)
{
    Windows::UI::Xaml::FocusState focusState = EnumerateInterfacesButton->FocusState;
    EnumerateInterfacesButton->IsEnabled = false;

    DeviceInterfacesOutputList->Items->Clear();

    try
    {
        auto selector = "System.Devices.InterfaceClassGuid:=\"" + InterfaceClassGuid->Text + "\"";
        //                 + " AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True";

        task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(selector, nullptr))
            .then([this](DeviceInformationCollection^ interfaces)
        {
            rootPage->NotifyUser(interfaces->Size + " device interface(s) found\n\n", NotifyType::StatusMessage);

            std::for_each(begin(interfaces), end(interfaces),
                [this](DeviceInformation^ deviceInterface)
            {
                DisplayDeviceInterface(deviceInterface);
            });

        });
    }
    catch (InvalidArgumentException^)
    {
        //The InvalidArgumentException gets thrown by FindAllAsync when the GUID isn't formatted properly
        //The only reason we're catching it here is because the user is allowed to enter GUIDs without validation
        //In normal usage of the API, this exception handling probably wouldn't be necessary when using known-good GUIDs

        rootPage->NotifyUser("Caught ArgumentException. Verify that you've entered a valid interface class GUID.", NotifyType::ErrorMessage);
    }

    EnumerateInterfacesButton->IsEnabled = true;
    EnumerateInterfacesButton->Focus(focusState);
}

void Interfaces::DisplayDeviceInterface(DeviceInformation^ deviceInterface)
{
    String^ id = "Id: " + deviceInterface->Id;
    String^ name = deviceInterface->Name;
    String^ isEnabled = (deviceInterface->IsEnabled) ? "IsEnabled: True" : "IsEnabled: False";

    auto item = ref new InterfaceDisplayItem(id, name, isEnabled);

    DeviceInterfacesOutputList->Items->Append(item);

    task<DeviceThumbnail^>(deviceInterface->GetThumbnailAsync())
        .then([item](DeviceThumbnail^ thumbnail)
    {
        item->Thumbnail->SetSource(thumbnail);
    });

    task<DeviceThumbnail^>(deviceInterface->GetGlyphThumbnailAsync())
        .then([item](DeviceThumbnail^ glyphThumbnail)
    {
        item->GlyphThumbnail->SetSource(glyphThumbnail);
    });
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Interfaces::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}
