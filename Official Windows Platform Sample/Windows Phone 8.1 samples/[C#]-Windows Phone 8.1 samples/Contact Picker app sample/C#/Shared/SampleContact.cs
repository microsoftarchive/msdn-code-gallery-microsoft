// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;

namespace SDKTemplate
{
    /// <summary>
    /// Sample contact.
    /// </summary>
    public class SampleContact
    {
        /// <summary>
        ///  Initializes a new instance of the SampleContact class.
        /// </summary>
        public SampleContact()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; private set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomeEmail { get; set; }

        public string WorkEmail { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string MobilePhone { get; set; }

        public string DisplayName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        /// <summary>
        /// Creates list of sample contacts.
        /// </summary>
        /// <returns>List of sample contacts</returns>
        public static List<SampleContact> CreateSampleContacts()
        {
            List<SampleContact> contacts = new List<SampleContact>
            {
                new SampleContact()
                {
                    FirstName = "David",
                    LastName = "Jaffe",
                    HomeEmail = "david@contoso.com",
                    WorkEmail = "david@cpandl.com",
                    MobilePhone = "248-555-0150",
                },
                new SampleContact()
                {
                    FirstName = "Kim",
                    LastName = "Abercrombie",
                    HomeEmail = "kim@contoso.com",
                    WorkEmail = "kim@adatum.com",
                    HomePhone = "444 555-0001",
                    WorkPhone = "245 555-0123",
                    MobilePhone = "921 555-0187",
                },
                new SampleContact()
                {
                    FirstName = "Jeff",
                    LastName = "Phillips",
                    HomeEmail = "jeff@contoso.com",
                    WorkEmail = "jeff@fabrikam.com",
                    HomePhone = "987-555-0199",
                    MobilePhone = "543-555-0111",
                },
                new SampleContact()
                {
                    FirstName = "Arlene",
                    LastName = "Huff",
                    HomeEmail = "arlene@contoso.com",
                    MobilePhone = "234-555-0156"
                },
                new SampleContact()
                {
                    FirstName = "Miles",
                    LastName = "Reid",
                    HomeEmail = "miles@contoso.com",
                    WorkEmail = "miles@proseware.com",
                    MobilePhone = "234-555-0154",
                }
            };

            return contacts;
        }
    }
}
