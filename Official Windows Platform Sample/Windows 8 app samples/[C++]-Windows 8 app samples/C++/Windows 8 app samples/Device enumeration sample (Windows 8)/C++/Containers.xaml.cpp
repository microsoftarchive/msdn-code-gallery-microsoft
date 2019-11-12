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
// Containers.xaml.cpp
// Implementation of the Containers class
//

#include "pch.h"
#include "Containers.xaml.h"

using namespace SDKSample::DeviceEnumeration;

Containers::Containers()
{
    InitializeComponent();
}

void Containers::EnumerateDeviceContainers(Object^ sender, RoutedEventArgs^ eventArgs)
{
    Windows::UI::Xaml::FocusState focusState = EnumerateContainersButton->FocusState;
    EnumerateContainersButton->IsEnabled = false;

    DeviceContainersOutputList->Items->Clear();

    auto properties = ref new Vector<String^>();
    properties->Append("System.ItemNameDisplay");
    properties->Append("System.Devices.ModelName");
    properties->Append("System.Devices.Connected");

    task<PnpObjectCollection^>(
        PnpObject::FindAllAsync(PnpObjectType::DeviceContainer, properties))
        .then([this](PnpObjectCollection^ containers)
    {
        rootPage->NotifyUser(containers->Size + " device container(s) found\n\n", NotifyType::StatusMessage);
;

        std::for_each(begin(containers), end(containers),
            [this](PnpObject^ container)
        {
            DisplayDeviceContainer(container);
        });
    });

    EnumerateContainersButton->IsEnabled = true;
    EnumerateContainersButton->Focus(focusState);
}

void Containers::DisplayDeviceContainer(PnpObject^ container)
{
    String^ id = "Id: " + container->Id;

    String^ name = dynamic_cast<String^>(container->Properties->Lookup("System.ItemNameDisplay"));
    if (name == nullptr)
    {
        name = "*Unnamed*";
    }

    String^ properties = "Property store count is: " +
        container->Properties->Size + "\n";

    std::for_each(begin(container->Properties), end(container->Properties),
        [&properties](IKeyValuePair<String^, Object^>^ prop)
    {
        properties += prop->Key + ": ";
        if (prop->Value != nullptr)
        {
            properties += prop->Value->ToString();
        }
        properties += "\n";
    });

    auto item = ref new ContainerDisplayItem(id, name, properties);

    DeviceContainersOutputList->Items->Append(item);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Containers::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}
