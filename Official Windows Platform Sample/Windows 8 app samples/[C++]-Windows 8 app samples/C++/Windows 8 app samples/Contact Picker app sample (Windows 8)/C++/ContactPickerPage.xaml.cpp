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
// ContactPickerPage.xaml.cpp
// Implementation of the ContactPickerPage class
//

#include "pch.h"
#include "ContactPickerPage.xaml.h"
#include "MainPagePicker.xaml.h"

using namespace ContactPicker;

using namespace Platform;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::ApplicationModel::Contacts::Provider;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls;

ContactPickerPage::ContactPickerPage()
{
    InitializeComponent();
    contactPickerUI = MainPagePicker::Current->contactPickerUI;

    for each (SampleContact contact in contactSet)
    {
        ListBoxItem^ item = ref new ListBoxItem();
        item->Name = contact.Id;
        item->Content = contact.Name;
        item->Tag = contact;
        ContactList->Items->Append(item);
    }
}

void ContactPickerPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = contactPickerUI->ContactRemoved += ref new TypedEventHandler<ContactPickerUI^, ContactRemovedEventArgs^>(this, &ContactPickerPage::OnContactRemoved, CallbackContext::Same);
}

void ContactPickerPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    contactPickerUI->ContactRemoved -= token;
}

void ContactPickerPage::OnContactRemoved(ContactPickerUI^ sender, ContactRemovedEventArgs^ e)
{
    for (unsigned int i = 0; i < ContactList->SelectedItems->Size; i++)
    {
        SampleContact contact = ResolveSampleContact(ContactList->SelectedItems->GetAt(i));
        if (e->Id == contact.Id)
        {
            ContactList->SelectedItems->RemoveAt(i);
            OutputText->Text += "\n" + contact.Name + " was removed from the basket";
            break;
        }
    }
}

void ContactPickerPage::ContactList_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    for each (Object^ added in e->AddedItems)
    {
        AddSampleContact(ResolveSampleContact(added));
    }

    for each (Object^ removed in e->RemovedItems)
    {
        SampleContact contact = ResolveSampleContact(removed);
        if (contactPickerUI->ContainsContact(contact.Id))
        {
            contactPickerUI->RemoveContact(contact.Id);
            OutputText->Text = contact.Name + " was removed from the basket";
        }
    }
}

void AppendValue(Contact^ contact, String^ value, ContactFieldType type, ContactFieldCategory category)
{
    if (!value->IsEmpty())
    {
        contact->Fields->Append(ref new ContactField(value, type, category));
    }
}

void ContactPickerPage::AddSampleContact(ContactPickerPage::SampleContact sampleContact)
{
    Contact^ contact = ref new Contact();
    contact->Name = sampleContact.Name;

    AppendValue(contact, sampleContact.HomeEmail, ContactFieldType::Email, ContactFieldCategory::Home);
    AppendValue(contact, sampleContact.WorkEmail, ContactFieldType::Email, ContactFieldCategory::Work);

    AppendValue(contact, sampleContact.HomePhone, ContactFieldType::PhoneNumber, ContactFieldCategory::Home);
    AppendValue(contact, sampleContact.MobilePhone, ContactFieldType::PhoneNumber, ContactFieldCategory::Mobile);
    AppendValue(contact, sampleContact.WorkPhone, ContactFieldType::PhoneNumber, ContactFieldCategory::Work);

    if (!sampleContact.Address->IsEmpty())
    {
        contact->Fields->Append(ref new ContactLocationField(sampleContact.Address, ContactFieldCategory::None,
            sampleContact.Street, sampleContact.City, sampleContact.State, "", sampleContact.ZipCode));
    }

    switch (contactPickerUI->AddContact(sampleContact.Id, contact))
    {
        case AddContactResult::Added:
            // Notify the user that the contact was added
            OutputText->Text = sampleContact.Name + " was added to the basket";
            break;
        case AddContactResult::AlreadyAdded:
            // Notify the user that the contact is already added
            OutputText->Text = sampleContact.Name + " is already in the basket";
            break;
        case AddContactResult::Unavailable:
        default:
            // Notify the user that the basket is unavailable
            OutputText->Text = sampleContact.Name + " could not be added to the basket";
            break;
    }
}

ContactPickerPage::SampleContact ContactPickerPage::ResolveSampleContact(Object^ listBoxItem)
{
    return safe_cast<SampleContact>(safe_cast<ListBoxItem^>(listBoxItem)->Tag);
}

// Sample set of contacts to pick from
Array<ContactPickerPage::SampleContact>^ ContactPickerPage::contactSet = ref new Array<ContactPickerPage::SampleContact>
{
    {
        /*Name*/ "David Jaffe",
        /*HomeEmail*/ "david@contoso.com",
        /*WorkEmail*/ "david@cpandl.com",
        /*HomePhone*/ "248-555-0150",
        /*WorkPhone*/ "",
        /*MobilePhone*/ "",
        /*Address*/ "3456 Broadway Ln, Los Angeles, CA",
        /*Street*/ "",
        /*City*/ "",
        /*State*/ "",
        /*ZipCode*/ "",
        /*Id*/ "761cb6fb-0270-451e-8725-bb575eeb24d5"
    },

    {
        /*Name*/ "Kim Abercrombie",
        /*HomeEmail*/ "kim@contoso.com",
        /*WorkEmail*/ "kim@adatum.com",
        /*HomePhone*/ "444 555-0001",
        /*WorkPhone*/ "245 555-0123",
        /*MobilePhone*/ "921 555-0187",
        /*Address*/ "123 Main St, Redmond, WA 23456",
        /*Street*/ "123 Main St",
        /*City*/ "Redmond",
        /*State*/ "WA",
        /*ZipCode*/ "23456",
        /*Id*/ "49b0652e-8f39-48c5-853b-e5e94e6b8a11"
    },

    {
        /*Name*/ "Jeff Phillips",
        /*HomeEmail*/ "jeff@contoso.com",
        /*WorkEmail*/ "jeff@fabrikam.com",
        /*HomePhone*/ "987-555-0199",
        /*WorkPhone*/ "",
        /*MobilePhone*/ "543-555-0111",
        /*Address*/ "456 2nd Ave, Dallas, TX 12345",
        /*Street*/ "456 2nd Ave",
        /*City*/ "Dallas",
        /*State*/ "TX",
        /*ZipCode*/ "12345",
        /*Id*/ "864abfb4-8998-4355-8236-8d69e47ec832"
    },

    {
        /*Name*/ "Arlene Huff",
        /*HomeEmail*/ "arlene@contoso.com",
        /*WorkEmail*/ "",
        /*HomePhone*/ "",
        /*WorkPhone*/ "",
        /*MobilePhone*/ "234-555-0156",
        /*Address*/ "",
        /*Street*/ "",
        /*City*/ "",
        /*State*/ "",
        /*ZipCode*/ "",
        /*Id*/ "27347af8-0e92-45b8-b14c-dd70fcd3b4a6"
    },

    {
        /*Name*/ "Miles Reid",
        /*HomeEmail*/ "miles@contoso.com",
        /*WorkEmail*/ "miles@proseware.com",
        /*HomePhone*/ "",
        /*WorkPhone*/ "",
        /*MobilePhone*/ "",
        /*Address*/ "678 Elm St, New York, New York 95111",
        /*Street*/ "678 Elm St",
        /*City*/ "New York",
        /*State*/ "New York",
        /*ZipCode*/ "95111",
        /*Id*/ "e3d24a99-0e29-41af-9add-18f5e3cfc518"
    }
};
