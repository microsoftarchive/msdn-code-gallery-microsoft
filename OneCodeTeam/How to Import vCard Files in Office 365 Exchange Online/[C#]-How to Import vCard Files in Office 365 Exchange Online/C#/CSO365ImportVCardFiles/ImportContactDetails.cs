/****************************** Module Header ******************************\
Module Name:  ImportContactDetails.cs
Project:      CSO365ImportVCardFiles
Copyright (c) Microsoft Corporation.

The vCard file format is supported by many email clients and email services. 
Now Outlook Web App supports to import the single .CSV file only. In this 
application, we will demonstrate how to import multiple vCard files in 
Office 365 Exchange Online.
The class includes the methods that are invoked in storing information.

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
using System.Text;

namespace CSO365ImportVCardFiles
{
    public static class ImportContactDetails
    {
        /// <summary>
        /// Store the Telephone information
        /// </summary>
        public static void ImportTelephone(Dictionary<ContactSchemaProperties, String> contactInfo,
            String keyName, String keyValue)
        {
            String telType = keyName.Split(';')[1];
            switch (telType)
            {
                case "WORK": if (!contactInfo.ContainsKey(ContactSchemaProperties.BusinessPhone))
                    {
                        contactInfo.Add(ContactSchemaProperties.BusinessPhone, keyValue);
                    }
                    else
                    {
                        contactInfo.Add(ContactSchemaProperties.BusinessPhone2, keyValue);
                    }
                    break;
                case "HOME": if (!contactInfo.ContainsKey(ContactSchemaProperties.HomePhone))
                    {
                        contactInfo.Add(ContactSchemaProperties.HomePhone, keyValue);
                    }
                    else
                    {
                        contactInfo.Add(ContactSchemaProperties.HomePhone2, keyValue);
                    }
                    break;
                case "CELL": contactInfo.Add(ContactSchemaProperties.MobilePhone, keyValue);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Store the Address information
        /// </summary>
        public static void ImportAddress(Dictionary<ContactSchemaProperties, String> contactInfo,
            String keyName, String keyValue)
        {
            String addrType = keyName.Split(';')[1];
            switch (addrType)
            {
                case "WORK":
                    {
                        String[] bussinessAdr = keyValue.Split(';');
                        if (bussinessAdr.Length >= 1)
                        {
                            contactInfo.Add(ContactSchemaProperties.OfficeLocation, bussinessAdr[1]);
                        }

                        ContactSchemaProperties[] properties = 
                        { 
                        ContactSchemaProperties.BusinessAddressStreet,
                        ContactSchemaProperties.BusinessAddressCity,
                        ContactSchemaProperties.BusinessAddressState,
                        ContactSchemaProperties.BusinessAddressPostalCode,
                        ContactSchemaProperties.BusinessAddressCountryOrRegion};

                        for (Int32 i = 2; i < bussinessAdr.Length; i++)
                        {
                            contactInfo.Add(properties[i - 2], bussinessAdr[i]);
                        }
                    }
                    break;
                case "HOME":
                    {
                        String[] homeAdr = keyValue.Split(';');

                        ContactSchemaProperties[] properties = { 
                                ContactSchemaProperties.HomeAddressStreet,
                                ContactSchemaProperties.HomeAddressCity,
                                ContactSchemaProperties.HomeAddressState,
                                ContactSchemaProperties.HomeAddressPostalCode,
                                ContactSchemaProperties.HomeAddressCountryOrRegion};

                        for (Int32 i = 2; i < homeAdr.Length; i++)
                        {
                            contactInfo.Add(properties[i - 2], homeAdr[i]);
                        }
                    }
                    break;
                case "POSTAL":
                    {
                        String[] postalAdr = keyValue.Split(';');

                        ContactSchemaProperties[] properties = { 
                                ContactSchemaProperties.OtherAddressStreet,
                                ContactSchemaProperties.OtherAddressCity,
                                ContactSchemaProperties.OtherAddressState,
                                ContactSchemaProperties.OtherAddressPostalCode,
                                ContactSchemaProperties.OtherAddressCountryOrRegion};

                        for (Int32 i = 2; i < postalAdr.Length; i++)
                        {
                            contactInfo.Add(properties[i - 2], postalAdr[i]);
                        }
                    }
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Store the Email information
        /// </summary>
        public static void ImportEmail(Dictionary<ContactSchemaProperties, String> contactInfo, 
            String keyName, String keyValue)
        {
            if (!contactInfo.ContainsKey(ContactSchemaProperties.EmailAddress1))
            {
                contactInfo.Add(ContactSchemaProperties.EmailAddress1, keyValue);
            }
            else if (!contactInfo.ContainsKey(ContactSchemaProperties.EmailAddress2))
            {
                contactInfo.Add(ContactSchemaProperties.EmailAddress2, keyValue);
            }
            else
            {
                contactInfo.Add(ContactSchemaProperties.EmailAddress3, keyValue);
            }
        }

        /// <summary>
        /// Store the Photo Information
        /// </summary>
        public static void ImportPhoto(Dictionary<ContactSchemaProperties, String> contactInfo, 
            String keyName, StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();

            if (!keyName.Contains("ENCODING"))
            {
                return;
            }
            else
            {
                String photoLine = reader.ReadLine();

                while (!String.IsNullOrWhiteSpace(photoLine))
                {
                    builder.Append(photoLine.Trim());

                    photoLine = reader.ReadLine();
                }

                if (builder.Length > 0)
                {
                    contactInfo.Add(ContactSchemaProperties.Photo, builder.ToString());
                }
            }
        }
    }
}
