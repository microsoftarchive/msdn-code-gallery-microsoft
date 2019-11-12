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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactPicker
{
    public sealed partial class ScenarioMultiple : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        public IReadOnlyList<ContactInformation> contacts;

        public ScenarioMultiple()
        {
            this.InitializeComponent();
            PickContactsButton.Click += PickContactsButton_Click;
        }

        private async void PickContactsButton_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.EnsureUnsnapped())
            {
                var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
                contactPicker.CommitButtonText = "Select";
                contacts = await contactPicker.PickMultipleContactsAsync();

                OutputContacts.Items.Clear();

                if (contacts.Count > 0)
                {
                    OutputEmpty.Visibility = Visibility.Collapsed;

                    foreach (ContactInformation contact in contacts)
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

    public class ContactItemAdapter
    {
        public string Name { get; private set; }
        public string SecondaryText { get; private set; }
        public BitmapImage Thumbnail { get; private set; }

        public ContactItemAdapter(ContactInformation contact)
        {
            Name = contact.Name;
            if (contact.Emails.Count > 0)
            {
                SecondaryText = contact.Emails[0].Value;
            }
            else if (contact.PhoneNumbers.Count > 0)
            {
                SecondaryText = contact.PhoneNumbers[0].Value;
            }
            else if (contact.Locations.Count > 0)
            {
                SecondaryText = contact.Locations[0].UnstructuredAddress;
            }
            GetThumbnail(contact);
        }

        private async void GetThumbnail(ContactInformation contact)
        {
            IRandomAccessStreamWithContentType stream = await contact.GetThumbnailAsync();
            if (stream != null && stream.Size > 0)
            {
                Thumbnail = new BitmapImage();
                Thumbnail.SetSource(stream);
            }
        }
    }
}
