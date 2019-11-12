// Copyright (c) Microsoft. All rights reserved.

using System;
using SDKTemplate;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ContactPicker
{
    /// <summary>
    /// ContactPickerPage class to select one or more contacts.
    /// </summary>
    public sealed partial class ContactPickerPage : Page
    {
        /// <summary>
        /// ContactPickerUI to select one or more contacts.
        /// </summary>
        private ContactPickerUI contactPickerUI = null;

        /// <summary>
        /// CoreDispatcher for the window.
        /// </summary>
        private CoreDispatcher dispatcher = Window.Current.Dispatcher;

        /// <summary>
        /// Initializes a new instance of the ContactPickerPage class.
        /// </summary>
        public ContactPickerPage()
        {
            this.InitializeComponent();
            ContactList.ItemsSource = SampleContact.CreateSampleContacts();
            ContactList.SelectionChanged += this.ContactList_SelectionChanged;
        }

        /// <summary>
        /// Activates ContactPickerPage.
        /// </summary>
        /// <param name="args">ContactPicker activated args</param>
        public void Activate(ContactPickerActivatedEventArgs args)
        {
            this.contactPickerUI = args.ContactPickerUI;
            Window.Current.Content = this;
            this.OnNavigatedTo(null);
            Window.Current.Activate();
        }

        /// <summary>
        /// Sets the event handler for when deselecting a contact. 
        /// </summary>
        /// <param name="e">Navigation event args</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.contactPickerUI.ContactRemoved += this.ContactPickerUI_ContactRemoved;
        }

        /// <summary>
        /// Removes the event handler for when deselecting a contact.
        /// </summary>
        /// <param name="e">Navigation event args</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.contactPickerUI.ContactRemoved -= this.ContactPickerUI_ContactRemoved;
        }

        /// <summary>
        /// Removes a contact from the ContactPickerUI.
        /// </summary>
        /// <param name="sender">ContactPickerIU to remove contact</param>
        /// <param name="args">Args of the removed contact</param>
        private async void ContactPickerUI_ContactRemoved(ContactPickerUI sender, ContactRemovedEventArgs args)
        {
            // The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
            string removedId = args.Id;
            await this.dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, 
                () =>
                {
                    foreach (SampleContact contact in ContactList.SelectedItems)
                    {
                        if (contact.Id == removedId)
                        {
                            ContactList.SelectedItems.Remove(contact);
                            OutputText.Text += "\n" + contact.DisplayName + " was removed from the basket";
                            break;
                        }
                    }
                });
        }

        /// <summary>
        /// Adds and removes a contact when the selection changes in ContactPickerUI.
        /// </summary>
        /// <param name="sender">ContactPickerUI to remove/add contact</param>
        /// <param name="e">Args of the selection changed</param>
        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (SampleContact added in e.AddedItems)
            {
                this.AddSampleContact(added);
            }

            foreach (SampleContact removed in e.RemovedItems)
            {
                if (this.contactPickerUI.ContainsContact(removed.Id))
                {
                    this.contactPickerUI.RemoveContact(removed.Id);
                    OutputText.Text = removed.DisplayName + " was removed from the basket";
                }
            }
        }

        /// <summary>
        /// Adds a contact to ContactPickerUI.
        /// </summary>
        /// <param name="sampleContact">Sample contact to add</param>
        private void AddSampleContact(SampleContact sampleContact)
        {
            Contact contact = new Contact();
            contact.Id = sampleContact.Id;
            contact.FirstName = sampleContact.FirstName;
            contact.LastName = sampleContact.LastName;

            if (!string.IsNullOrEmpty(sampleContact.HomeEmail))
            {
                ContactEmail homeEmail = new ContactEmail();
                homeEmail.Address = sampleContact.HomeEmail;
                homeEmail.Kind = ContactEmailKind.Personal;
                contact.Emails.Add(homeEmail);
            }

            if (!string.IsNullOrEmpty(sampleContact.WorkEmail))
            {
                ContactEmail workEmail = new ContactEmail();
                workEmail.Address = sampleContact.WorkEmail;
                workEmail.Kind = ContactEmailKind.Work;
                contact.Emails.Add(workEmail);
            }

            if (!string.IsNullOrEmpty(sampleContact.HomePhone))
            {
                ContactPhone homePhone = new ContactPhone();
                homePhone.Number = sampleContact.HomePhone;
                homePhone.Kind = ContactPhoneKind.Home;
                contact.Phones.Add(homePhone);
            }

            if (!string.IsNullOrEmpty(sampleContact.MobilePhone))
            {
                ContactPhone mobilePhone = new ContactPhone();
                mobilePhone.Number = sampleContact.MobilePhone;
                mobilePhone.Kind = ContactPhoneKind.Mobile;
                contact.Phones.Add(mobilePhone);
            }

            if (!string.IsNullOrEmpty(sampleContact.WorkPhone))
            {
                ContactPhone workPhone = new ContactPhone();
                workPhone.Number = sampleContact.WorkPhone;
                workPhone.Kind = ContactPhoneKind.Work;
                contact.Phones.Add(workPhone);
            }

            switch (this.contactPickerUI.AddContact(contact))
            {
                case AddContactResult.Added:
                    // Notify the user that the contact was added
                    OutputText.Text = contact.DisplayName + " was added to the basket";
                    break;
                case AddContactResult.AlreadyAdded:
                    // Notify the user that the contact is already added
                    OutputText.Text = contact.DisplayName + " is already in the basket";
                    break;
                case AddContactResult.Unavailable:
                default:
                    // Notify the user that the basket is unavailable
                    OutputText.Text = contact.DisplayName + " could not be added to the basket";
                    break;
            }
        }
    }
}