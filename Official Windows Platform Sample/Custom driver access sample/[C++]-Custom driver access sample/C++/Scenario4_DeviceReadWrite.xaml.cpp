//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include <sstream>

#include "Scenario4_DeviceReadWrite.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::CustomDeviceAccess;

using namespace Platform;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Documents;

using namespace Windows::Devices::Custom;
using namespace Windows::Storage::Streams;

using namespace Concurrency;

DeviceReadWrite::DeviceReadWrite() : writeCounter(0), readCounter(0)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceReadWrite::OnNavigatedTo(NavigationEventArgs ^ /* e */)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}

void DeviceReadWrite::ReadBlock_Click(Platform::Object ^ sender, Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();

    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    auto button = safe_cast<Button^>(sender);
    button->IsEnabled = false;

    // create a data reader and load data into it from the input stream of the 
    // device.
    auto dataReader = ref new DataReader(fx2Device->InputStream);

    int counter = readCounter++;

    LOG_MESSAGE(L"Read " << counter << L" begin");
    
    create_task(dataReader->LoadAsync(64)).
    then(
        [=] (unsigned int /*count*/) 
        {
            // Get the message out of the data reader
            auto message = dataReader->ReadString(dataReader->UnconsumedBufferLength);
            
            LOG_MESSAGE(L"Read " << counter << L" end: " << message->Data());
            
            button->IsEnabled = true;
        }
    );

    return;
}


void DeviceReadWrite::WriteBlock_Click(Platform::Object ^  sender , Windows::UI::Xaml::RoutedEventArgs ^ /* e */)
{
    auto fx2Device = DeviceList::Current->GetSelectedDevice();

    if (fx2Device == nullptr) {
        rootPage->NotifyUser("Fx2 device not connected or accessible", NotifyType::ErrorMessage);
        return;
    }

    auto button = safe_cast<Button^>(sender) ;
    button->IsEnabled = false;

    // Generate a string to write to the device
    auto counter = writeCounter++;

    auto dataWriter = ref new DataWriter();

    auto message = ref new String(L"This is message ") + counter;

    dataWriter->WriteString(message);

    LOG_MESSAGE(L"Write " << counter << L" begin: " << message->Data());

    create_task(fx2Device->OutputStream->WriteAsync(dataWriter->DetachBuffer())).
    then(
        [=](unsigned int bytesWritten)
        {
            LOG_MESSAGE(L"Write " << counter << L" end: " << bytesWritten << L" bytes written");
            button->IsEnabled = true;
        }
    );
}


void DeviceReadWrite::LogMessage(const std::wstringstream& stream)
{
    auto s = ref new String(stream.str().c_str());

    auto span = ref new Span();
    auto run = ref new Run();

    run->Text = s;
    
    span->Inlines->Append(run);
    span->Inlines->Append(ref new LineBreak());

    OutputText->Inlines->InsertAt(0, span);
}