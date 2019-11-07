//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include <ppltasks.h>
#include "ScenarioShowContactCardDelayLoad.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ContactManager;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Concurrency;

ScenarioShowContactCardDelayLoad::ScenarioShowContactCardDelayLoad()
{
    InitializeComponent();
}

IAsyncOperation<Contact^>^ SDKSample::ContactManager::ScenarioShowContactCardDelayLoad::DownloadContactDataAsync(Contact^ contact)
{
    return create_async([contact]() -> Contact^
    {
        // Simulate the download latency by delaying the execution by 2 seconds.
        wait(2000);

        // Add more data to the contact object.
        ContactEmail^ workEmail = ref new ContactEmail();
        workEmail->Address = "kim@adatum.com";
        workEmail->Kind = ContactEmailKind::Work;
        contact->Emails->Append(workEmail);

        ContactPhone^ homePhone = ref new ContactPhone();
        homePhone->Number = "(444) 555-0001";
        homePhone->Kind = ContactPhoneKind::Home;
        contact->Phones->Append(homePhone);

        ContactPhone^ workPhone = ref new ContactPhone();
        workPhone->Number = "(245) 555-0123";
        workPhone->Kind = ContactPhoneKind::Work;
        contact->Phones->Append(workPhone);

        ContactPhone^ mobilePhone = ref new ContactPhone();
        mobilePhone->Number = "(921) 555-0187";
        mobilePhone->Kind = ContactPhoneKind::Mobile;
        contact->Phones->Append(mobilePhone);

        ContactAddress^ address = ref new ContactAddress();
        address->StreetAddress = "123 Main St";
        address->Locality = "Redmond";
        address->Region = "WA";
        address->Country = "USA";
        address->PostalCode = "23456";
        address->Kind = ContactAddressKind::Home;
        contact->Addresses->Append(address);

        return contact;
    });
}

void SDKSample::ContactManager::ScenarioShowContactCardDelayLoad::ShowContactCardDelayLoadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create contact object with small set of initial data to display.
    Contact^ contact = ref new Contact();
    contact->FirstName = "Kim";
    contact->LastName = "Abercrombie";

    ContactEmail^ email = ref new ContactEmail();
    email->Address = "kim@contoso.com";
    contact->Emails->Append(email);

    // Get the selection rect of the button pressed to show contact card.
    Rect rect = GetElementRect(safe_cast<FrameworkElement^>(sender));
    ContactCardDelayedDataLoader^ dataLoader = Windows::ApplicationModel::Contacts::ContactManager::ShowDelayLoadedContactCard(
        contact, 
        rect, 
        Windows::UI::Popups::Placement::Below // The contact card placement can change when it is updated with more data. For improved user experience, specify placement 
                                              // of the card so that it has space to grow and will not need to be repositioned. In this case, default placement first places 
                                              // the card above the button because the card is small, but after the card is updated with more data, the operating system moves 
                                              // the card below the button to fit the card's expanded size. Specifying that the contact card is placed below at the beginning 
                                              // avoids this repositioning.
        );

    Platform::String^ message = "ContactManager::ShowDelayLoadedContactCard() was called.\r\n";
    MainPage::Current->NotifyUser(message, NotifyType::StatusMessage);

    // Simulate downloading more data from the network for the contact.
    message += "Downloading data ...\r\n";
    MainPage::Current->NotifyUser(message, NotifyType::StatusMessage);

    create_task(DownloadContactDataAsync(contact)).then([dataLoader, message](Contact^ fullContact)
    {
        if (fullContact)
        {
            // We get more data - update the contact card with the full set of contact data.
            dataLoader->SetData(fullContact);

            Platform::String^ messageLambda = message;
            messageLambda += "ContactCardDelayedDataLoader.SetData() was called.\r\n";
            MainPage::Current->NotifyUser(messageLambda, NotifyType::StatusMessage);
        }
        else
        {
            // No more data to show - delete the data loader to trigger the call to IClosable::Close() to tell the contact card UI all data has been set.
            delete dataLoader;
        }
    });
}
