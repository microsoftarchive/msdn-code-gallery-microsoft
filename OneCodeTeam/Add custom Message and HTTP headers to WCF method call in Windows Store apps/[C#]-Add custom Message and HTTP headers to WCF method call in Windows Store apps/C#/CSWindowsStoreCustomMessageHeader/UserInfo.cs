/****************************** Module Header ******************************\
 * Module Name:  UserInfo.cs
 * Project:      CSWindowsStoreCustomMessageHeader
 * Copyright (c) Microsoft Corporation.
 * 
 *  This is a UserInfo class. It will be passed in as a MessageHeader.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


namespace CSWindowsStoreCustomMessageHeader
{
    public sealed class UserInfo
    {
        private string _firstName;
        private string _lastName;
        private int _age;

        /// <summary>
        /// FirstName of the user.
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        /// <summary>
        /// LastName of the user.
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Age of the user.
        /// </summary>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
    }
}
