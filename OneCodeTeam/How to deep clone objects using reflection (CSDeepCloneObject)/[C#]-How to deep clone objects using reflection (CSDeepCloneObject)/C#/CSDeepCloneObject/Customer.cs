/****************************** Module Header ******************************\
Module Name:  Customer.cs
Project:      CSDeepCloneObject
Copyright (c) Microsoft Corporation.

The struct is used to demonstrate the deep clone.

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
   struct Customer
    {
        private Int32 id;
        private String name;
        private Address address;

        public Int32 Id
        {
            get { return id; }
            set { id = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Address Address
        {
            get { return address; }
            set { address = value; }
        }
    }
}
