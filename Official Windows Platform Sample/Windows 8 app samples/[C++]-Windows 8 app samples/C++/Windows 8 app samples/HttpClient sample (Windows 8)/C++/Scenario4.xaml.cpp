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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::HttpClientSample::Scenario4::Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // 'BaseAddress' is a disabled text box, so the value is considered trusted input. When enabling
    // the text box make sure to validate the string (e.g., by using Uri.TryCreate()).
    Uri^ baseAddress = ref new Uri(BaseAddress->Text);

    Helpers::ScenarioStarted(StartButton, CancelButton);
    rootPage->NotifyUser("In progress", NotifyType::StatusMessage);
    OutputField->Text = "";

    // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
    // text box make sure to validate user input (e.g., by catching exceptions with TryGetUri as shown in scenario 1).
    Uri^ resourceAddress = baseAddress->CombineUri(AddressField->Text);

    cancellationTokenSource = cancellation_token_source();

    // Do an asynchronous POST.  We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    httpRequest.PostAsync(resourceAddress, RequestBodyField->Text->Data(),
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

void SDKSample::HttpClientSample::Scenario4::Cancel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    cancellationTokenSource.cancel();
}

void SDKSample::HttpClientSample::Scenario4::DisplayTextResult(const std::wstring& response, TextBox^ output)
{
    output->Text += httpRequest.GetStatusCode() + " " + ref new String(httpRequest.GetReasonPhrase().c_str()) + "\n";

    // Convert all instances of <br> to newline.
    std::wstring ws = response;
    Helpers::ReplaceAll(ws, L"<br>", L"\n");

    output->Text += ref new String(ws.c_str());
}
