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

using namespace SDKSample::AtomPub;

using namespace Windows::UI::Xaml::Controls;
using namespace Concurrency;
using namespace Windows::Web;

Scenario4::Scenario4()
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
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser().
    rootPage = MainPage::Current;

    CommonData::Restore(this);
}

void Scenario4::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

// Download a feed.
void Scenario4::GetFeed_Click(Object^ sender, RoutedEventArgs^ e)
{
    OutputField->Text = "";

    // If we retrieve the feed via the Edit uri then we will be logged in and be
    // able to modify/delete the resource.

    // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate it
    // by using TryGetUri().
    // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
    // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
    // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
    // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
    Uri^ resourceUri;
    if (!rootPage->TryGetUri(StringHelper::Trim(ServiceAddressField->Text) + CommonData::EditUri, &resourceUri))
    {
        return;
    }

    rootPage->NotifyUser("Fetching feed...", NotifyType::StatusMessage);
    OutputField->Text = "Requested feed: " + resourceUri->DisplayUri + "\r\n";

    task<SyndicationFeed^>(CommonData::GetClient()->RetrieveFeedAsync(resourceUri)).then([this] (SyndicationFeed^ feed)
    {
        this->feedIterator->AttachFeed(feed);

        rootPage->NotifyUser("Fetching feed completed.", NotifyType::StatusMessage);

        if (this->feedIterator->HasElements())
        {
            OutputField->Text += "Got feed\r\n";
            OutputField->Text += "Title: " + this->feedIterator->GetTitle() + "\r\n";
            OutputField->Text += "EditUri: " + this->feedIterator->GetEditUri()->DisplayUri + "\r\n";

            DisplayItem();
        }
        else
        {
            OutputField->Text += "Got empty feed\r\n";
        }
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

// Update the current item.
void Scenario4::UpdateItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (!this->feedIterator->HasElements())
    {
        rootPage->NotifyUser("No item currently displayed, please download a feed first.", NotifyType::ErrorMessage);
        return;
    }

    rootPage->NotifyUser("Updating item...", NotifyType::StatusMessage);
    OutputField->Text += "Item location: " + feedIterator->GetEditUri()->DisplayUri + "\r\n";

    // Update the item.
    SyndicationItem^ updatedItem = ref new SyndicationItem();
    updatedItem->Title = ref new SyndicationText(TitleBox->Text, SyndicationTextType::Text);
    updatedItem->Content = ref new SyndicationContent(ContentBox->Text, SyndicationTextType::Html);

    task<void>(CommonData::GetClient()->UpdateResourceAsync(this->feedIterator->GetEditUri(), updatedItem)).then([this] (task<void> t)
    {
        try
        {
            t.get();
            
            rootPage->NotifyUser("Item updated.", NotifyType::StatusMessage);
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

void Scenario4::PreviousItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    this->feedIterator->MovePrevious();
    DisplayItem();
}

void Scenario4::NextItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    this->feedIterator->MoveNext();
    DisplayItem();
}

void Scenario4::UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario4::PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario4::DisplayItem()
{
    IndexField->Text = this->feedIterator->GetIndexDescription();
    TitleBox->Text = this->feedIterator->GetTitle();
    ContentBox->Text = feedIterator->GetContent();
}
