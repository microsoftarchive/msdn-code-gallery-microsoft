// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.Phone.PersonalInformation;

namespace SDKTemplate
{
    /// <summary>
    /// Sets phone configuration.
    /// </summary>
    public static class PhoneConfiguration
    {
        /// <summary>
        /// Creates a contact store and add contacts.
        /// </summary>
        public static async void CreateContactStore()
        {
            ContactStore contactStore = await ContactStore.CreateOrOpenAsync(
                            ContactStoreSystemAccessMode.ReadWrite,
                            ContactStoreApplicationAccessMode.ReadOnly);

            foreach (SampleContact sampleContact in SampleContact.CreateSampleContacts())
            {
                StoredContact contact = new StoredContact(contactStore);
                IDictionary<string, object> props = await contact.GetPropertiesAsync();

                if (!string.IsNullOrEmpty(sampleContact.FirstName))
                {
                    props.Add(KnownContactProperties.GivenName, sampleContact.FirstName);
                }

                if (!string.IsNullOrEmpty(sampleContact.LastName))
                {
                    props.Add(KnownContactProperties.FamilyName, sampleContact.LastName);
                }

                if (!string.IsNullOrEmpty(sampleContact.HomeEmail))
                {
                    props.Add(KnownContactProperties.Email, sampleContact.HomeEmail);
                }

                if (!string.IsNullOrEmpty(sampleContact.WorkEmail))
                {
                    props.Add(KnownContactProperties.WorkEmail, sampleContact.WorkEmail);
                }

                if (!string.IsNullOrEmpty(sampleContact.HomePhone))
                {
                    props.Add(KnownContactProperties.Telephone, sampleContact.HomePhone);
                }

                if (!string.IsNullOrEmpty(sampleContact.WorkPhone))
                {
                    props.Add(KnownContactProperties.CompanyTelephone, sampleContact.WorkPhone);
                }

                if (!string.IsNullOrEmpty(sampleContact.MobilePhone))
                {
                    props.Add(KnownContactProperties.MobileTelephone, sampleContact.MobilePhone);
                }

                await contact.SaveAsync();
            }
        }
    }
}