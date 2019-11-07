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

using namespace SDKSample::AtomPub;

using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Web::Syndication;
using namespace Concurrency;

Scenario2::Scenario2()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser().
    rootPage = MainPage::Current;

    CommonData::Restore(this);
}

void Scenario2::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

// Submit an item.
void Scenario2::SubmitItem_Click(Object^ sender, RoutedEventArgs^ e)
{
    OutputField->Text = "";

    // The title cannot be an empty string or a string with white spaces only, since it is used also
    // as the resource description (Slug header).
    if (StringHelper::Trim(TitleField->Text) == "")
    {
        rootPage->NotifyUser("Post title cannot be blank.", NotifyType::ErrorMessage);
        return;
    }

    // Since the value of 'ServiceAddressField' was provided by the user it is untrusted input. We'll validate it
    // by using TryGetUri().
    // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
    // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
    // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
    // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
    Uri^ serviceUri;
    if (!rootPage->TryGetUri(StringHelper::Trim(ServiceAddressField->Text) + CommonData::ServiceDocUri, &serviceUri))
    {
        return;
    }

    rootPage->NotifyUser("Performing operation...", NotifyType::StatusMessage);
    OutputField->Text = "Fetching Service document: " + serviceUri->DisplayUri + "\r\n";

    task<ServiceDocument^>(CommonData::GetClient()->RetrieveServiceDocumentAsync(serviceUri)).then([this] (ServiceDocument^ serviceDocument)
    {
        // The result here is usually the same as:
        // Uri resourceUri = new Uri(StringHelper::Trim(ServiceAddressField->Text) + Common->EditUri);
        // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
        // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
        // with servers on the intErnet, the "Home or Work Networking" capability should not be set.
        Uri^ resourceUri = FindEditUri(serviceDocument);
        if (resourceUri == nullptr)
        {
            cancel_current_task();
        }

        OutputField->Text += "Uploading Post: " + resourceUri->DisplayUri + "\r\n";

        SyndicationItem^ item = ref new SyndicationItem();
        item->Title = ref new SyndicationText(TitleField->Text, SyndicationTextType::Text);
        item->Content = ref new SyndicationContent(ContentField->Text, SyndicationTextType::Html);

        return CommonData::GetClient()->CreateResourceAsync(resourceUri, item->Title->Text, item);
    }).then([this] (SyndicationItem^ result)
    {
        rootPage->NotifyUser("New post created.", NotifyType::StatusMessage);
        OutputField->Text += "Posted at " + result->ItemUri->DisplayUri + "\r\n";
    }).then([this] (task<void> t)
    {
        try
        {
            t.get();
        }
        catch (task_canceled&)
        {
            rootPage->NotifyUser("URI not found in service document.", NotifyType::ErrorMessage);
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

// Read the service document to find the URI we're supposed to use when uploading content.
Uri^ Scenario2::FindEditUri(ServiceDocument^ serviceDocument)
{
    for (unsigned int i = 0; i < serviceDocument->Workspaces->Size; i++)
    {
        Workspace^ workspace = serviceDocument->Workspaces->GetAt(i);

        for (unsigned int j = 0; j < workspace->Collections->Size; j++)
        {
            ResourceCollection^ collection = workspace->Collections->GetAt(j);

            if (StringHelper::Join(";", collection->Accepts) == "application/atom+xml;type=entry")
            {
                return collection->Uri;
            }
        }
    }

    return nullptr;
}

void Scenario2::UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}

void Scenario2::PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e)
{
    CommonData::Save(ServiceAddressField->Text, UserNameField->Text, PasswordField->Password);
}
