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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace concurrency;
using namespace Web;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::HttpClientSample::Scenario1::Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OutputField->Text = "";

    // The value of 'AddressField' is set by the user and is therefore untrusted input.
    // The URI is validated by calling TryGetUri() that will return 'false' for strings that are not valid URIs.
    // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
    // since the user may provide URIs for servers located on the intErnet or intrAnet. If apps only
    // communicate with servers on the intErnet, only the "Internet (Client)" capability should be set.
    // Similarly if an app is only intended to communicate on the intrAnet, only the "Home and Work
    // Networking" capability should be set.
    Uri^ uri;
    if (!rootPage->TryGetUri(AddressField->Text, &uri))
    {
        return;
    }

    Helpers::ScenarioStarted(StartButton, CancelButton);
    rootPage->NotifyUser("In progress", NotifyType::StatusMessage);

    cancellationTokenSource = cancellation_token_source();

    // Do an asynchronous GET.  We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    httpRequest.GetAsync(uri, cancellationTokenSource.get_token()).then([this](task<std::wstring> response)
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

void SDKSample::HttpClientSample::Scenario1::Cancel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    cancellationTokenSource.cancel();
}

void SDKSample::HttpClientSample::Scenario1::DisplayTextResult(const std::wstring& response, TextBox^ output)
{
    output->Text += httpRequest.GetStatusCode() + " " + ref new String(httpRequest.GetReasonPhrase().c_str()) + "\n";

    // Convert all instances of <br> to newline.
    std::wstring ws = response;
    Helpers::ReplaceAll(ws, L"<br>", L"\n");

    output->Text += ref new String(ws.c_str());
}
