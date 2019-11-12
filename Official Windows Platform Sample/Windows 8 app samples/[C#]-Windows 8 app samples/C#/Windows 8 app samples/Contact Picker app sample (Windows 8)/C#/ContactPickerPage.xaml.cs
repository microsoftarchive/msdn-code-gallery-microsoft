//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using SDKTemplate;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ContactPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPickerPage : SDKTemplate.Common.LayoutAwarePage
    {
        ContactPickerUI contactPickerUI = MainPagePicker.Current.contactPickerUI;
        CoreDispatcher dispatcher = Window.Current.Dispatcher;

        public ContactPickerPage()
        {
            this.InitializeComponent();
            ContactList.ItemsSource = contactSet;
            ContactList.SelectionChanged += ContactList_SelectionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            contactPickerUI.ContactRemoved += contactPickerUI_ContactRemoved;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            contactPickerUI.ContactRemoved -= contactPickerUI_ContactRemoved;
        }

        async void contactPickerUI_ContactRemoved(ContactPickerUI sender, ContactRemovedEventArgs args)
        {
            // The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
            string removedId = args.Id;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (SampleContact contact in ContactList.SelectedItems)
                {
                    if (contact.Id == removedId)
                    {
                        ContactList.SelectedItems.Remove(contact);
                        OutputText.Text += "\n" + contact.Name + " was removed from the basket";
                        break;
                    }
                }
            });
        }

        void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (SampleContact added in e.AddedItems)
            {
                AddSampleContact(added);
            }

            foreach (SampleContact removed in e.RemovedItems)
            {
                if (contactPickerUI.ContainsContact(removed.Id))
                {
                    contactPickerUI.RemoveContact(removed.Id);
                    OutputText.Text = removed.Name + " was removed from the basket";
                }
            }
        }

        void AppendValue(Contact contact, string value, ContactFieldType type, ContactFieldCategory category)
        {
            if (!string.IsNullOrEmpty(value))
            {
                contact.Fields.Add(new ContactField(value, type, category));
            }
        }

        void AddSampleContact(SampleContact sampleContact)
        {
            Contact contact = new Contact();
            contact.Name = sampleContact.Name;

            AppendValue(contact, sampleContact.HomeEmail, ContactFieldType.Email, ContactFieldCategory.Home);
            AppendValue(contact, sampleContact.WorkEmail, ContactFieldType.Email, ContactFieldCategory.Work);

            AppendValue(contact, sampleContact.HomePhone, ContactFieldType.PhoneNumber, ContactFieldCategory.Home);
            AppendValue(contact, sampleContact.MobilePhone, ContactFieldType.PhoneNumber, ContactFieldCategory.Mobile);
            AppendValue(contact, sampleContact.WorkPhone, ContactFieldType.PhoneNumber, ContactFieldCategory.Work);

            if (!string.IsNullOrEmpty(sampleContact.Address))
            {
                contact.Fields.Add(new ContactLocationField(sampleContact.Address, ContactFieldCategory.None,
                    sampleContact.Street, sampleContact.City, sampleContact.State, "", sampleContact.ZipCode));
            }

            switch (contactPickerUI.AddContact(sampleContact.Id, contact))
            {
                case AddContactResult.Added:
                    // Notify the user that the contact was added
                    OutputText.Text = sampleContact.Name + " was added to the basket";
                    break;
                case AddContactResult.AlreadyAdded:
                    // Notify the user that the contact is already added
                    OutputText.Text = sampleContact.Name + " is already in the basket";
                    break;
                case AddContactResult.Unavailable:
                default:
                    // Notify the user that the basket is unavailable
                    OutputText.Text = sampleContact.Name + " could not be added to the basket";
                    break;
            }
        }

        // Example contacts to pick from
        List<SampleContact> contactSet = new List<SampleContact>()
        {
            new SampleContact()
            {
                Name = "David Jaffe",
                HomeEmail = "david@contoso.com",
                WorkEmail = "david@cpandl.com",
                HomePhone = "248-555-0150",
                Address = "3456 Broadway Ln, Los Angeles, CA",
                Street = "",
                City = "",
                State = "",
                ZipCode = ""
            },
            new SampleContact()
            {
                Name = "Kim Abercrombie",
                HomeEmail = "kim@contoso.com",
                WorkEmail = "kim@adatum.com",
                HomePhone = "444 555-0001",
                WorkPhone = "245 555-0123",
                MobilePhone = "921 555-0187",
                Address = "123 Main St, Redmond, WA 23456",
                Street = "123 Main St",
                City = "Redmond",
                State = "WA",
                ZipCode = "23456"
            },
            new SampleContact()
            {
                Name = "Jeff Phillips",
                HomeEmail = "jeff@contoso.com",
                WorkEmail = "jeff@fabrikam.com",
                HomePhone = "987-555-0199",
                MobilePhone = "543-555-0111",
                Address = "456 2nd Ave, Dallas, TX 12345",
                Street = "456 2nd Ave",
                City = "Dallas",
                State = "TX",
                ZipCode = "12345"
            },
            new SampleContact()
            {
                Name = "Arlene Huff",
                HomeEmail = "arlene@contoso.com",
                MobilePhone = "234-555-0156"
            },
            new SampleContact()
            {
                Name = "Miles Reid",
                HomeEmail = "miles@contoso.com",
                WorkEmail = "miles@proseware.com",
                Address = "678 Elm St, New York, New York 95111",
                Street = "678 Elm St",
                City = "New York",
                State = "New York",
                ZipCode = "95111"
            }
        };

    }

    internal class SampleContact
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string HomeEmail { get; set; }
        public string WorkEmail { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }

        public string Address { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public SampleContact()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

}
