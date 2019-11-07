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

using namespace SDKSample;

using namespace Platform;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::ApplicationModel::Contacts::Provider;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls;

/// <summary>
/// Initializes a new instance of the ContactPickerPage class.
/// </summary>
ContactPickerPage::ContactPickerPage()
{
    InitializeComponent();

	for (SampleContact contact : ContactCreator::CreateSampleContacts())
    {
        ListBoxItem^ item = ref new ListBoxItem();
        item->Name = contact.Id;
        item->Content = contact.DisplayName;
        item->Tag = contact;
        ContactList->Items->Append(item);
    }
}

/// <summary>
/// Activates ContactPickerPage.
/// </summary>
/// <param name="args">ContactPicker activated args</param>
void ContactPickerPage::Activate(Windows::ApplicationModel::Activation::ContactPickerActivatedEventArgs^ args)
{
	contactPickerUI = args->ContactPickerUI;
	Window::Current->Content = this;
	this->OnNavigatedTo(nullptr);
	Window::Current->Activate();
}

/// <summary>
/// Sets the event handler for when deselecting a contact. 
/// </summary>
/// <param name="e">Navigation event args</param>
void ContactPickerPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = contactPickerUI->ContactRemoved += ref new TypedEventHandler<ContactPickerUI^, ContactRemovedEventArgs^>(this, &ContactPickerPage::OnContactRemoved, CallbackContext::Same);
}

/// <summary>
/// Removes the event handler for when deselecting a contact.
/// </summary>
/// <param name="e">Navigation event args</param>
void ContactPickerPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    contactPickerUI->ContactRemoved -= token;
}

/// <summary>
/// Removes a contact from the ContactPickerUI.
/// </summary>
/// <param name="sender">ContactPickerIU to remove contact</param>
/// <param name="args">Args of the removed contact</param>
void ContactPickerPage::OnContactRemoved(ContactPickerUI^ sender, ContactRemovedEventArgs^ e)
{
    for (unsigned int i = 0; i < ContactList->SelectedItems->Size; i++)
    {
        SampleContact contact = ResolveSampleContact(ContactList->SelectedItems->GetAt(i));
        if (e->Id == contact.Id)
        {
            ContactList->SelectedItems->RemoveAt(i);
            OutputText->Text += "\n" + contact.DisplayName + " was removed from the basket";
            break;
        }
    }
}

/// <summary>
/// Adds and removes a contact when the selection changes in ContactPickerUI.
/// </summary>
/// <param name="sender">ContactPickerUI to remove/add contact</param>
/// <param name="e">Args of the selection changed</param>
void ContactPickerPage::ContactList_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    for (Object^ added : e->AddedItems)
    {
        AddSampleContact(ResolveSampleContact(added));
    }

    for (Object^ removed : e->RemovedItems)
    {
        SampleContact contact = ResolveSampleContact(removed);
        if (contactPickerUI->ContainsContact(contact.Id))
        {
            contactPickerUI->RemoveContact(contact.Id);
            OutputText->Text = contact.DisplayName + " was removed from the basket";
        }
    }
}

/// <summary>
/// Adds a contact to ContactPickerUI.
/// </summary>
/// <param name="sampleContact">Sample contact to add</param>
void ContactPickerPage::AddSampleContact(SampleContact sampleContact)
{
    Contact^ contact = ref new Contact();
    contact->Id = sampleContact.Id;
    contact->FirstName = sampleContact.FirstName;
    contact->LastName = sampleContact.LastName;
    
    if (!sampleContact.HomeEmail->IsEmpty())
    {
        ContactEmail^ homeEmail = ref new ContactEmail();
        homeEmail->Address = sampleContact.HomeEmail;
        homeEmail->Kind = ContactEmailKind::Personal;
        contact->Emails->Append(homeEmail);
    }

    if (!sampleContact.WorkEmail->IsEmpty())
    {
        ContactEmail^ workEmail = ref new ContactEmail();
        workEmail->Address = sampleContact.WorkEmail;
        workEmail->Kind = ContactEmailKind::Work;
        contact->Emails->Append(workEmail);
    }

    if (!sampleContact.HomePhone->IsEmpty())
    {
        ContactPhone^ homePhone = ref new ContactPhone();
        homePhone->Number = sampleContact.HomePhone;
        homePhone->Kind = ContactPhoneKind::Home;
        contact->Phones->Append(homePhone);
    }

    if (!sampleContact.MobilePhone->IsEmpty())
    {
        ContactPhone^ mobilePhone = ref new ContactPhone();
        mobilePhone->Number = sampleContact.MobilePhone;
        mobilePhone->Kind = ContactPhoneKind::Mobile;
        contact->Phones->Append(mobilePhone);
    }

    if (!sampleContact.WorkPhone->IsEmpty())
    {
        ContactPhone^ workPhone = ref new ContactPhone();
        workPhone->Number = sampleContact.WorkPhone;
        workPhone->Kind = ContactPhoneKind::Work;
        contact->Phones->Append(workPhone);
    }

    switch (contactPickerUI->AddContact(contact))
    {
        case AddContactResult::Added:
            // Notify the user that the contact was added
            OutputText->Text = contact->DisplayName + " was added to the basket";
            break;
        case AddContactResult::AlreadyAdded:
            // Notify the user that the contact is already added
            OutputText->Text = contact->DisplayName + " is already in the basket";
            break;
        case AddContactResult::Unavailable:
        default:
            // Notify the user that the basket is unavailable
            OutputText->Text = contact->DisplayName + " could not be added to the basket";
            break;
    }
}

/// <summary>
/// Returns the sample contact in the list box.
/// </summary>
/// <param name="listBoxItem">Item in the list box</param>
SampleContact ContactPickerPage::ResolveSampleContact(Object^ listBoxItem)
{
    return safe_cast<SampleContact>(safe_cast<ListBoxItem^>(listBoxItem)->Tag);
}