/****************************** Module Header ******************************\
Module Name:  ContactsHelper.cs
Project:      CSO365ExportContacts
Copyright (c) Microsoft Corporation.

Outlook Web App (OWA) allows us to import multiple contacts in a very simple 
way. However, it does not allow to export contacts. In this application, we 
will demonstrate how to export contacts from Office 365 Exchange Online.
This file includes the helper methods of contact.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.WebServices.Data;

namespace CSO365ExportContacts
{
    public static class ContactsHelper
    {
        /// <summary>
        /// Get the contact proerties that you want to write. 
        /// </summary>
        public static Dictionary<PropertyDefinitionBase, String> GetSchemaList()
        {
            // Key is the property definition, and the value is the column title of the CSV file.
            Dictionary<PropertyDefinitionBase, String> schemaList = 
                new Dictionary<PropertyDefinitionBase, string>();
            schemaList.Add(ContactSchema.Surname, "Last Name");
            schemaList.Add(ContactSchema.GivenName, "First Name");
            schemaList.Add(ContactSchema.CompanyName, "Company");
            schemaList.Add(ContactSchema.Department, "Department");
            schemaList.Add(ContactSchema.JobTitle, "Job Title");
            schemaList.Add(ContactSchema.BusinessPhone, "Business Phone");
            schemaList.Add(ContactSchema.HomePhone, "Home Phone");
            schemaList.Add(ContactSchema.MobilePhone, "Mobile Phone");
            schemaList.Add(ContactSchema.BusinessAddressStreet, "Business Street");
            schemaList.Add(ContactSchema.BusinessAddressCity, "Business City");
            schemaList.Add(ContactSchema.BusinessAddressState, "Business State");
            schemaList.Add(ContactSchema.BusinessAddressPostalCode, "Business Postal Code");
            schemaList.Add(ContactSchema.BusinessAddressCountryOrRegion, "Business Country/Region");
            schemaList.Add(ContactSchema.HomeAddressStreet, "Home Street");
            schemaList.Add(ContactSchema.HomeAddressCity, "Home City");
            schemaList.Add(ContactSchema.HomeAddressState, "Home State");
            schemaList.Add(ContactSchema.HomeAddressPostalCode, "Home Postal Code");
            schemaList.Add(ContactSchema.HomeAddressCountryOrRegion, "Home Country/Region");
            schemaList.Add(ContactSchema.EmailAddress1, "Email Address");

            return schemaList;
        }

        /// <summary>
        /// Write the contacts into the CSV file.
        /// </summary>
        public static void WriteContacts(StreamWriter writer, PropertyDefinitionBase proerty, Contact contact)
        {
            if (proerty.Equals(ContactSchema.Surname))
            {
                if (!String.IsNullOrWhiteSpace(contact.Surname))
                {
                    writer.Write("\"{0}\"", contact.Surname);
                }
            }
            else if (proerty.Equals(ContactSchema.GivenName))
            {
                if (!String.IsNullOrWhiteSpace(contact.GivenName))
                {
                    writer.Write("\"{0}\"", contact.GivenName);
                }
            }
            else if (proerty.Equals(ContactSchema.CompanyName))
            {
                if (!String.IsNullOrWhiteSpace(contact.CompanyName))
                {
                    writer.Write("\"{0}\"", contact.CompanyName);
                }
            }
            else if (proerty.Equals(ContactSchema.Department))
            {
                if (!String.IsNullOrWhiteSpace(contact.Department))
                {
                    writer.Write("\"{0}\"", contact.Department);
                }
            }
            else if (proerty.Equals(ContactSchema.JobTitle))
            {
                if (!String.IsNullOrWhiteSpace(contact.JobTitle))
                {
                    writer.Write("\"{0}\"", contact.JobTitle);
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessPhone))
            {
                if (contact.PhoneNumbers.Contains(PhoneNumberKey.BusinessPhone))
                {
                    if (!String.IsNullOrWhiteSpace(contact.PhoneNumbers[PhoneNumberKey.BusinessPhone]))
                    {
                        writer.Write("\"{0}\"", contact.PhoneNumbers[PhoneNumberKey.BusinessPhone]);
                    }
                }
            }
            else if (proerty.Equals(ContactSchema.HomePhone))
            {
                if (contact.PhoneNumbers.Contains(PhoneNumberKey.HomePhone))
                {
                    if (!String.IsNullOrWhiteSpace(contact.PhoneNumbers[PhoneNumberKey.HomePhone]))
                    {
                        writer.Write("\"{0}\"", contact.PhoneNumbers[PhoneNumberKey.HomePhone]);
                    }
                }
            }
            else if (proerty.Equals(ContactSchema.MobilePhone))
            {
                if (contact.PhoneNumbers.Contains(PhoneNumberKey.MobilePhone))
                {
                    if (!String.IsNullOrWhiteSpace(contact.PhoneNumbers[PhoneNumberKey.MobilePhone]))
                    {
                        writer.Write("\"{0}\"", contact.PhoneNumbers[PhoneNumberKey.MobilePhone]);
                    }
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessAddressStreet))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business))
                {
                    if (!String.IsNullOrWhiteSpace(contact.PhysicalAddresses[PhysicalAddressKey.Business].Street))
                    {
                        writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Business].Street);
                    }
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessAddressCity))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Business].City);
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessAddressState))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Business].State);
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessAddressPostalCode))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Business].PostalCode);
                }
            }
            else if (proerty.Equals(ContactSchema.BusinessAddressCountryOrRegion))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business))
                {
                    writer.Write("\"{0}\"", 
                        contact.PhysicalAddresses[PhysicalAddressKey.Business].CountryOrRegion);
                }
            }
            else if (proerty.Equals(ContactSchema.HomeAddressStreet))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Home].Street);
                }
            }
            else if (proerty.Equals(ContactSchema.HomeAddressCity))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Home].City);
                }
            }
            else if (proerty.Equals(ContactSchema.HomeAddressState))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Home].State);
                }
            }
            else if (proerty.Equals(ContactSchema.HomeAddressPostalCode))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Home].PostalCode);
                }
            }
            else if (proerty.Equals(ContactSchema.HomeAddressCountryOrRegion))
            {
                if (contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home))
                {
                    writer.Write("\"{0}\"", contact.PhysicalAddresses[PhysicalAddressKey.Home].CountryOrRegion);
                }
            }
            else if (proerty.Equals(ContactSchema.EmailAddress1))
            {
                if (contact.EmailAddresses.Contains(EmailAddressKey.EmailAddress1))
                {
                    writer.Write("\"{0}\"", contact.EmailAddresses[EmailAddressKey.EmailAddress1].Address);
                }
            }
        }

    }
}
