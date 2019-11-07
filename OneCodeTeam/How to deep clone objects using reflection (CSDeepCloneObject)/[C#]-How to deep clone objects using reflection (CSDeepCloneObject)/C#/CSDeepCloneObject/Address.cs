/****************************** Module Header ******************************\
Module Name:  Address.cs
Project:      CSDeepCloneObject
Copyright (c) Microsoft Corporation.

The class contains the address information.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace CSDeepCloneObject
{
    class Address
    {
        private String addressLine;
        private String city;
        private String postalCode;

        public String AddressLine
        {
            get { return addressLine; }
            set { addressLine = value; }
        }

        public String City
        {
            get { return city; }
            set { city = value; }
        }

        public String PostalCode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }
    }
}
