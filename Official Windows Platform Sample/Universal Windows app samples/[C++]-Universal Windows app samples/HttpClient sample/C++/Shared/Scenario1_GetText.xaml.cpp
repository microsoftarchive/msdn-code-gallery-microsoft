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
#include "Scenario1_GetText.xaml.h"

using namespace SDKSample::HttpClientSample;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;

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

    filter = ref new HttpBaseProtocolFilter();
    httpClient = ref new HttpClient(filter);
    cancellationTokenSource = cancellation_token_source();
}

void Scenario1::Start_Click(Object^ sender, RoutedEventArgs^ e)
{
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

    Helpers::ScenarioStarted(StartButton, CancelButton, OutputField);
    rootPage->NotifyUser("In progress", NotifyType::StatusMessage);

    if (ReadDefaultRadio->IsChecked->Value)
    {
        filter->CacheControl->ReadBehavior = HttpCacheReadBehavior::Default;
    }
    else if (ReadMostRecentRadio->IsChecked->Value)
    {
        filter->CacheControl->ReadBehavior = HttpCacheReadBehavior::MostRecent;
    }
    else if (ReadOnlyFromCacheRadio->IsChecked->Value)
    {
        filter->CacheControl->ReadBehavior = HttpCacheReadBehavior::OnlyFromCache;
    }

    if (WriteDefaultRadio->IsChecked->Value)
    {
        filter->CacheControl->WriteBehavior = HttpCacheWriteBehavior::Default;
    }
    else if (WriteNoCacheRadio->IsChecked->Value)
    {
        filter->CacheControl->WriteBehavior = HttpCacheWriteBehavior::NoCache;
    }

    // Do an asynchronous GET. We need to use use_current() with the continuations since the tasks are completed on
    // background threads and we need to run on the UI thread to update the UI.
    create_task(httpClient->GetAsync(uri), cancellationTokenSource.get_token()).then([=](HttpResponseMessage^ response)
    {
        return Helpers::DisplayTextResultAsync(response, OutputField, cancellationTokenSource.get_token());
    }, task_continuation_context::use_current()).then([=](task<HttpResponseMessage^> previousTask)
    {
        try
        {
            // Check if any previous task threw an exception.
            HttpResponseMessage^ response = previousTask.get();

            rootPage->NotifyUser(
                "Completed. Response came from " + response->Source.ToString() + ".",
                NotifyType::StatusMessage);
        }
        catch (const task_canceled&)
        {
            rootPage->NotifyUser("Request canceled.", NotifyType::ErrorMessage);
        }
        catch (Exception^ ex)
        {
            rootPage->NotifyUser("Error: " + ex->Message, NotifyType::ErrorMessage);
        }

        Helpers::ScenarioCompleted(StartButton, CancelButton);
    });
}

void Scenario1::Cancel_Click(Object^ sender, RoutedEventArgs^ e)
{
    cancellationTokenSource.cancel();

    // Re-create the CancellationTokenSource.
    cancellationTokenSource = cancellation_token_source();
}
