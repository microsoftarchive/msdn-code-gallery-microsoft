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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::HttpClientSample::Scenario5::Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Helpers::ScenarioStarted(StartButton, CancelButton);
    rootPage->NotifyUser("In progress", NotifyType::StatusMessage);
    OutputField->Text = "";

    // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
    // text box make sure to validate user input (e.g., by catching exceptions with TryGetUri as shown in scenario 1).
    Uri^ resourceAddress = ref new Uri(AddressField->Text);

    ComPtr<IStream> stream;
    const unsigned int contentLength = 1000;
    stream.Attach(GenerateSampleStream(contentLength));

    cancellationTokenSource = cancellation_token_source();

    // Do an asynchronous POST.  We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    httpRequest.PostAsync(resourceAddress, nullptr, stream.Get(), contentLength,
        cancellationTokenSource.get_token()).then([this](task<std::wstring> response)
    {
        Helpers::ScenarioCompleted(StartButton, CancelButton);

        try
        {
            DisplayTextResult(response.get(), OutputField);
            rootPage->NotifyUser("Completed", NotifyType::StatusMessage);
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

IStream* SDKSample::HttpClientSample::Scenario5::GenerateSampleStream(unsigned int size)
{
    ComPtr<IStream> postStream;
    Web::HttpRequest::CreateMemoryStream(&postStream);

    // Generate sample data.
    std::unique_ptr<byte> subData(new byte[size]);

    for (unsigned int i = 0; i < size; i++)
    {
        subData.get()[i] = '@';
    }

    CheckHResult(postStream->Write(subData.get(), size, nullptr));

    return postStream.Detach();
}

void SDKSample::HttpClientSample::Scenario5::Cancel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    cancellationTokenSource.cancel();
}

void SDKSample::HttpClientSample::Scenario5::DisplayTextResult(const std::wstring& response, TextBox^ output)
{
    output->Text += httpRequest.GetStatusCode() + " " + ref new String(httpRequest.GetReasonPhrase().c_str()) + "\n";

    // Convert all instances of <br> to newline.
    std::wstring ws = response;
    Helpers::ReplaceAll(ws, L"<br>", L"\n");

    output->Text += ref new String(ws.c_str());
}
