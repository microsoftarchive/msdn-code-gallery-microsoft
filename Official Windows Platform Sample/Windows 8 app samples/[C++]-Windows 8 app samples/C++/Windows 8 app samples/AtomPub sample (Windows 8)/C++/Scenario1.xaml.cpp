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

using namespace SDKSample::AtomPub;

using namespace Windows::UI::Xaml::Controls;
using namespace Concurrency;

Scenario1::Scenario1()
{
    InitializeComponent();

    // Helps iterating through feeds.
    this->feedIterator = ref new SyndicationItemIterator();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser().
    rootPage = MainPage::Current;

    CommonData::Restore(this);
}

void Scenario1::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario1::GetFeed_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Note that this feed is public by default and will not require authentication.
    // We will only get back a limited use feed, without information about editing.
    
    // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate it
    // by using TryGetUri().
    // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
    // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
    // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
    // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
    Uri^ resourceUri;
    if (!rootPage->TryGetUri(StringHelper::Trim(ServiceAddressField->Text) + CommonData::FeedUri, &resourceUri))
    {
        return;
    }

    rootPage->NotifyUser("Fetching resource...", NotifyType::StatusMessage);
    OutputField->Text = "Requested resource: " + resourceUri->DisplayUri + "\r\n";

    task<SyndicationFeed^>(CommonData::GetClient()->RetrieveFeedAsync(resourceUri)).then([this] (SyndicationFeed^ feed)
    {
        this->feedIterator->AttachFeed(feed);

        rootPage->NotifyUser("Fetching resource completed.", NotifyType::StatusMessage);
        DisplayItem();
    }).then([this] (task<void> t)
    {
        try
        {
            t.get();
        }
        catch (Exception^ exception)
        {
            if (!rootPage->HandleException(exception, OutputField))
            {
                throw;
            }
        }
    });
}

void Scenario1::PreviousItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    this->feedIterator->MovePrevious();
    DisplayItem();
}

void Scenario1::NextItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    this->feedIterator->MoveNext();
    DisplayItem();
}

void Scenario1::UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario1::PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario1::DisplayItem()
{
    IndexField->Text = this->feedIterator->GetIndexDescription();
    TitleField->Text = this->feedIterator->GetTitle();
    ContentWebView->NavigateToString(this->feedIterator->GetContent());
}
