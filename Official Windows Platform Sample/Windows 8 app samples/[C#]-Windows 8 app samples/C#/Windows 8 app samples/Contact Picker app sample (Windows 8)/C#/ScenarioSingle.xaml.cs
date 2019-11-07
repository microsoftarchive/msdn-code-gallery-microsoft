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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioSingle : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public ScenarioSingle()
        {
            this.InitializeComponent();
            PickAContactButton.Click += PickAContactButton_Click;
        }

        private async void PickAContactButton_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.EnsureUnsnapped())
            {
                var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
                contactPicker.CommitButtonText = "Select";
                ContactInformation contact = await contactPicker.PickSingleContactAsync();

                if (contact != null)
                {
                    OutputFields.Visibility = Visibility.Visible;
                    OutputEmpty.Visibility = Visibility.Collapsed;

                    OutputName.Text = contact.Name;
                    AppendContactFieldValues(OutputEmailHeader, OutputEmails, contact.Emails);
                    AppendContactFieldValues(OutputPhoneNumberHeader, OutputPhoneNumbers, contact.PhoneNumbers);
                    AppendContactFieldValues(OutputAddressHeader, OutputAddresses, contact.Locations);

                    IRandomAccessStreamWithContentType stream = await contact.GetThumbnailAsync();
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
                else
                {
                    OutputEmpty.Visibility = Visibility.Visible;
                    OutputFields.Visibility = Visibility.Collapsed;
                    OutputThumbnail.Source = null;
                }
            }
        }

        private void AppendContactFieldValues<T>(TextBlock header, TextBlock content, IReadOnlyCollection<T> fields)
        {
            if (fields.Count > 0)
            {
                StringBuilder output = new StringBuilder();
                foreach (IContactField field in fields)
                {
                    if (field.Type == ContactFieldType.Location)
                    {
                        AppendLocationValue(output, (ContactLocationField)field);
                    }
                    else
                    {
                        output.AppendFormat("{0} ({1})\n", field.Value, field.Category);
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

        private void AppendLocationValue(StringBuilder output, ContactLocationField location)
        {
            string address = location.UnstructuredAddress;
            if (string.IsNullOrEmpty(address))
            {
                List<String> parts = (new List<string> { location.Street, location.City, location.Region, location.Country, location.PostalCode });
                address = string.Join(", ", parts.FindAll(s => !string.IsNullOrEmpty(s)));
            }
            output.AppendFormat("{0} ({1})\n", address, location.Category);
        }
    }
}
