/****************************** Module Header ******************************\
 * Module Name:  Person.cs
 * Project:      CSUnvsAppJsonToWebService
 * Copyright (c) Microsoft Corporation.
 * 
 * This is post data class
 *  
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System.Runtime.Serialization;

namespace CSUnvsAppJsonToWebService
{
    [DataContract]
    public class Person
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        public int Age { get; set; }
    }
}
