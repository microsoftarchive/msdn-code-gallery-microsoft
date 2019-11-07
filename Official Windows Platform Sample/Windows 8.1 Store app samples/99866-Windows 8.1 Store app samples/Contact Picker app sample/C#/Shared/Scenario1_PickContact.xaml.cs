// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using SDKTemplate;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactPicker
{
    /// <summary>
    /// Feature scenario to select a single contact.
    /// </summary>
    public sealed partial class Scenario1_PickContact : Page
    {
        /// <summary>
        /// Initializes a new instance of the Scenario1_PickContact class. 
        /// </summary>
        public Scenario1_PickContact()
        {
            this.InitializeComponent();
            PickAContactButton.Click += this.PickAContactButton_Click;
#if WINDOWS_PHONE_APP
            PhoneConfiguration.CreateContactStore();
#endif 
        }

        /// <summary>
        /// Pick a single contact. 
        /// </summary>
        /// <param name="sender">Click sender</param>
        /// <param name="e">Routed event args</param>
        private async void PickAContactButton_Click(object sender, RoutedEventArgs e)
        {
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email);
            Contact contact = await contactPicker.PickContactAsync();

            if (contact != null)
            {
                OutputFields.Visibility = Visibility.Visible;
                OutputEmpty.Visibility = Visibility.Collapsed;

                OutputName.Text = contact.DisplayName;
                this.AppendContactFieldValues(this.OutputEmailHeader, this.OutputEmails, contact.Emails);
                this.AppendContactFieldValues(this.OutputPhoneNumberHeader, this.OutputPhoneNumbers, contact.Phones);

                if (contact.Thumbnail != null)
                {
                    IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
                    if (stream != null && stream.Size > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(stream);
                        OutputThumbnail.Source = bitmap;
                    }
                    else
                    {
                        OutputThumbnail.Source = null;
                    }
                }
            }
            else
            {
                OutputEmpty.Visibility = Visibility.Visible;
                OutputFields.Visibility = Visibility.Collapsed;
                OutputThumbnail.Source = null;
            }            
        }

        /// <summary>
        /// Appends contact field values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">Contact field header</param>
        /// <param name="content">Contact field content</param>
        /// <param name="fields">Contact fields</param>
        private void AppendContactFieldValues<T>(TextBlock header, TextBlock content, IList<T> fields)
        {
            if (fields.Count > 0)
            {
                StringBuilder output = new StringBuilder();
                if (fields[0].GetType() == typeof(ContactEmail))
                {
                    foreach (ContactEmail email in fields as IList<ContactEmail>)
                    {
                        output.AppendFormat("Email Address: {0} ({1})\n", email.Address, email.Kind);
                    }
                }
                else if (fields[0].GetType() == typeof(ContactPhone))
                {
                    foreach (ContactPhone phone in fields as IList<ContactPhone>)
                    {
                        output.AppendFormat("Phone: {0} ({1})\n", phone.Number, phone.Kind);
                    }
                }
                
                header.Visibility = Visibility.Visible;
                content.Visibility = Visibility.Visible;
                content.Text = output.ToString();
            }
            else
            {
                header.Visibility = Visibility.Collapsed;
                content.Visibility = Visibility.Collapsed;
            }
        }
    }
}
