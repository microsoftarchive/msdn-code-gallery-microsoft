/****************************** Module Header ******************************\
Module Name:  SetContactDetails.cs
Project:      CSO365ImportVCardFiles
Copyright (c) Microsoft Corporation.

The vCard file format is supported by many email clients and email services. 
Now Outlook Web App supports to import the single .CSV file only. In this 
application, we will demonstrate how to import multiple vCard files in 
Office 365 Exchange Online.
The class includes the methods that are invoked in set contact properties.


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace CSO365ImportVCardFiles
{
    public static class SetContactDetails
    {
        /// <summary>
        /// Set the address entry.
        /// </summary>
        public static void SetAddress(ContactSchemaProperties key, String keyValue, 
            PhysicalAddressEntry addressEntry)
        {
            switch (key)
            {
                case ContactSchemaProperties.BusinessAddressStreet:
                case ContactSchemaProperties.HomeAddressStreet:
                case ContactSchemaProperties.OtherAddressStreet:
                    addressEntry.Street = keyValue;
                    break;
                case ContactSchemaProperties.BusinessAddressCity:
                case ContactSchemaProperties.HomeAddressCity:
                case ContactSchemaProperties.OtherAddressCity:
                    addressEntry.City = keyValue;
                    break;
                case ContactSchemaProperties.BusinessAddressState:
                case ContactSchemaProperties.HomeAddressState:
                case ContactSchemaProperties.OtherAddressState:
                    addressEntry.State = keyValue;
                    break;
                case ContactSchemaProperties.BusinessAddressPostalCode:
                case ContactSchemaProperties.HomeAddressPostalCode:
                case ContactSchemaProperties.OtherAddressPostalCode:
                    addressEntry.PostalCode = keyValue;
                    break;
                case ContactSchemaProperties.BusinessAddressCountryOrRegion:
                case ContactSchemaProperties.HomeAddressCountryOrRegion:
                case ContactSchemaProperties.OtherAddressCountryOrRegion:
                    addressEntry.CountryOrRegion = keyValue;
                    break;
                default:
                    break;
            }
        }
    }
}
