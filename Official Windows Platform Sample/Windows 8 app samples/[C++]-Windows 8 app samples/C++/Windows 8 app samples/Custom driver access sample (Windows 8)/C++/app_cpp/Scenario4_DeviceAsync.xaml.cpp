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
// DeviceAsync.xaml.cpp
// Implementation of the DeviceAsync class
//

#include "pch.h"
#include "Scenario4_DeviceAsync.xaml.h"

using namespace SDKSample::CustomDeviceAccess;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

DeviceAsync::DeviceAsync()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceAsync::OnNavigatedTo(NavigationEventArgs^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}


void SDKSample::CustomDeviceAccess::DeviceAsync::deviceAsyncSet_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    // Get the selector element
    unsigned int val = barGraphInput->SelectedIndex;
    auto barGraphArray = ref new Platform::Array<bool>(8);

    for (unsigned int i = 0; i < barGraphArray->Length; i += 1) {
        barGraphArray[i] = i < val;
    }

    try {
        DeviceList::Current->Fx2Device->SetBarGraphDisplay(barGraphArray);
    }
    catch (Platform::Exception^ exception) {
        rootPage->NotifyUser(exception->ToString(), NotifyType::ErrorMessage);
        return;
    }

    this->ClearBarGraphTable();
    rootPage->NotifyUser("Bar Graph Set to " + val, NotifyType::StatusMessage);
}

void SDKSample::CustomDeviceAccess::DeviceAsync::deviceAsyncGet_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    rootPage->NotifyUser("Getting Fx2 Bars", NotifyType::StatusMessage);

    // Start the async operation to retrieve the LEDs
    auto async = DeviceList::Current->Fx2Device->GetBarGraphDisplayAsync();

    // Create a task around the async operation we got back
    concurrency::task<Samples::Devices::Fx2::BarGraphDisplayResult^> result(async);

    // On completion handle either success or failure
    result.then(
        [this](concurrency::task<Samples::Devices::Fx2::BarGraphDisplayResult^> Result) {
            try {
                // get the result.  This throws if the async operation failed
                auto result = Result.get();
                UpdateBarGraphTable(result->BarGraphDisplay);
                rootPage->NotifyUser("Got bars value", NotifyType::StatusMessage);
            } catch (Platform::Exception^ exception) {
                rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
            }
        }
        );
}

void SDKSample::CustomDeviceAccess::DeviceAsync::ClearBarGraphTable(void)
{
    barGraphOutput->Inlines->Clear();
}

void SDKSample::CustomDeviceAccess::DeviceAsync::UpdateBarGraphTable(Platform::Array<bool>^ BarGraphArray) 
{
    auto output = barGraphOutput;

    DeviceList::CreateBooleanTable(
        output->Inlines,
        BarGraphArray,
        nullptr,
        "Bar Number",
        "Bar State",
        "Lit",
        "Not Lit"
        );
}
