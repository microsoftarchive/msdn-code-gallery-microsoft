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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Security::Cryptography;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();

    this->readBuffer = ref new Buffer(1000);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::HttpClientSample::Scenario2::Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Helpers::ScenarioStarted(StartButton, CancelButton);
    rootPage->NotifyUser("In progress", NotifyType::StatusMessage);
    OutputField->Text = "";

    // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
    // text box make sure to validate user input (e.g., by catching exceptions with TryGetUri as shown in scenario 1).
    auto resourceAddress = ref new Uri(AddressField->Text);

    cancellationTokenSource = cancellation_token_source();

    // Do an asynchronous GET.  We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    httpRequest.SendAsync(L"GET", resourceAddress, cancellationTokenSource.get_token()).then([this]()
    {
        OutputField->Text = httpRequest.GetStatusCode() + " " + 
            ref new String(httpRequest.GetReasonPhrase().c_str()) + "\n";

        return Scenario2ReadData();
    }, task_continuation_context::use_current()).then([this](task<void> previousTask)
    {
        rootPage->NotifyUser("Completed", NotifyType::StatusMessage);

        Helpers::ScenarioCompleted(StartButton, CancelButton);

        try
        {
            previousTask.get();
        }
        catch (const task_canceled&)
        {
            rootPage->NotifyUser("Request canceled.", NotifyType::ErrorMessage);
        }
        catch (Exception^ ex)
        {
            rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
        }
    }, task_continuation_context::use_current());
}

task<void> SDKSample::HttpClientSample::Scenario2::Scenario2ReadData()
{
    // Do an asynchronous read.  We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    return httpRequest.ReadAsync(readBuffer, 0, readBuffer->Capacity).then([this](task<void> readTask)
    {
        // If the read failed, throw an exception so we don't start another read.
        readTask.get();

        OutputField->Text += "Bytes read from stream: " + readBuffer->Length + "\n"; 

        // Use the buffer contents for something.  We can't safely display it as a string though, since encodings
        // like UTF-8 and UTF-16 have a variable number of bytes per character and so the last bytes in the buffer
        // may not contain a whole character.   Instead, we'll convert the bytes to hex and display the result.
        String^ responseBodyAsText = CryptographicBuffer::EncodeToHexString(readBuffer);
        OutputField->Text += responseBodyAsText + "\n";

        // Continue reading until the response is complete.  When done, return previousTask that is complete.
        return httpRequest.IsResponseComplete() ? readTask : Scenario2ReadData();
    }, task_continuation_context::use_current());
}

void SDKSample::HttpClientSample::Scenario2::Cancel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    cancellationTokenSource.cancel();
}
