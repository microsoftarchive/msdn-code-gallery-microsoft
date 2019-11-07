// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactPicker
{
    /// <summary>
    /// Contact item adapter.
    /// </summary>
    public class ContactItemAdapter
    {
        /// <summary>
        /// Initializes a new instance of the ContactItemAdapter class.
        /// </summary>
        /// <param name="contact"></param>
        public ContactItemAdapter(Contact contact)
        {
            this.Name = contact.DisplayName;
            if (contact.Emails.Count > 0)
            {
                this.SecondaryText = contact.Emails[0].Address;
            }
            else if (contact.Phones.Count > 0)
            {
                this.SecondaryText = contact.Phones[0].Number;
            }

            this.GetThumbnail(contact);
        }

        public string Name { get; private set; }

        public string SecondaryText { get; private set; }

        public BitmapImage Thumbnail { get; private set; }

        /// <summary>
        /// Gets contact thumbnail.
        /// </summary>
        /// <param name="contact">Contact to get the thumbnail from</param>
        private async void GetThumbnail(Contact contact)
        {
            if (contact.Thumbnail != null)
            {
                IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
                if (stream != null && stream.Size > 0)
                {
                    this.Thumbnail = new BitmapImage();
                    this.Thumbnail.SetSource(stream);
                }
            }
        }
    }
}
