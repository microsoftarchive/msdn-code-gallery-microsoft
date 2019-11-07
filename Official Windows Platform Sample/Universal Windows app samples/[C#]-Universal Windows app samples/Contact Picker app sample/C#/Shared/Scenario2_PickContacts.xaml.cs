// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using SDKTemplate;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ContactPicker
{
    /// <summary>
    /// Feature scenario to select multiple contacts.
    /// </summary>
    public sealed partial class Scenario2_PickContacts : Page
    {
        private IList<Contact> contacts;

        /// <summary>
        /// Initializes a new instance of the Scenario2_PickContacts class. 
        /// </summary>
        public Scenario2_PickContacts()
        {
            this.InitializeComponent();
            this.PickContactsButton.Click += this.PickContactsButton_Click;
            OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#if WINDOWS_PHONE_APP
            PhoneConfiguration.CreateContactStore();
#endif 
        }

        /// <summary>
        /// Pick multiple contacts. 
        /// </summary>
        /// <param name="sender">Click sender</param>
        /// <param name="e">Routed event args</param>
        private async void PickContactsButton_Click(object sender, RoutedEventArgs e)
        {
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email);
            this.contacts = await contactPicker.PickContactsAsync();

            OutputContacts.Items.Clear();

            if (this.contacts.Count > 0)
            {
                OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Visible;
                OutputEmpty.Visibility = Visibility.Collapsed;

                foreach (Contact contact in this.contacts)
                {
                    OutputContacts.Items.Add(new ContactItemAdapter(contact));
                }
            }
            else
            {
                OutputEmpty.Visibility = Visibility.Visible;
            }         
        }
    }
}
