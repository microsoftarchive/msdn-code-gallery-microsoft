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
// DeviceProperties.xaml.cpp
// Implementation of the DeviceProperties class
//

#include "pch.h"
#include "Scenario2_DeviceProperties.xaml.h"

using namespace SDKSample::CustomDeviceAccess;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

DeviceProperties::DeviceProperties()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceProperties::OnNavigatedTo(NavigationEventArgs^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}


void DeviceProperties::devicePropertiesGet_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    try 
    {
        auto segment = DeviceList::Current->Fx2Device->SevenSegmentDisplay;
        rootPage->NotifyUser(
            "The segment display value is " + segment,
            NotifyType::StatusMessage
            );
    } catch (Platform::InvalidArgumentException^) {

        rootPage->NotifyUser(
            "The segment display is not yet initialized",
            NotifyType::StatusMessage
            );
    } catch (Platform::Exception^ exception) {
        rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
        return;
    }
}


void DeviceProperties::devicePropertiesSet_Click_1(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (DeviceList::Current->Fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    uint8 val = (uint8) (devicePropertiesSegmentInput->SelectedIndex + 1);

    try 
    {
        DeviceList::Current->Fx2Device->SevenSegmentDisplay = val;
    }
    catch (Platform::Exception^ exception) {
        rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
        return;
    }
}
