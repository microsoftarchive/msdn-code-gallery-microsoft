//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include <robuffer.h>

#include "Scenario2_DeviceIO.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::CustomDeviceAccess;

using namespace Platform;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Custom;
using namespace Windows::Storage::Streams;

using namespace Concurrency;

DeviceIO::DeviceIO()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceIO::OnNavigatedTo(NavigationEventArgs ^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}

void DeviceIO::DeviceIOGet_Click_1(Platform::Object ^ /* sender */, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();
    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    auto outputBuffer = ref new Buffer(1);

    create_task(fx2Device->SendIOControlAsync(
                            Fx2Driver::GetSevenSegmentDisplay,
                            nullptr,
                            outputBuffer
                            )).
    then(
        [=](task<unsigned int> result)
        {
            try 
            {
                result.get();

                auto data = GetArrayFromBuffer(outputBuffer);

                rootPage->NotifyUser(
                    "The segment display value is " + Fx2Driver::SevenSegmentToDigit(data[0]),
                    NotifyType::StatusMessage
                    );

            }
            catch (Exception^ exception)
            {
                rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
            }
        }
    );

    return;
}


void DeviceIO::DeviceIOSet_Click_1(Platform::Object ^ /* sender */, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();
    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    byte val = (byte) (DeviceIOSegmentInput->SelectedIndex + 1);

    auto inputBuffer = ref new Buffer(1);
    auto data = GetArrayFromBuffer(inputBuffer);
    data[0] = Fx2Driver::DigitToSevenSegment(val);
    inputBuffer->Length = 1;

    create_task(fx2Device->SendIOControlAsync(
                            Fx2Driver::SetSevenSegmentDisplay,
                            inputBuffer,
                            nullptr
                            )).
    then(
        [=](task<unsigned int> result) 
        {
            try
            {
                result.get();
            }
            catch (Platform::Exception^ exception) 
            {
                rootPage->NotifyUser(exception->Message, NotifyType::ErrorMessage);
                return;
            }
        }
    );

    return;
}

