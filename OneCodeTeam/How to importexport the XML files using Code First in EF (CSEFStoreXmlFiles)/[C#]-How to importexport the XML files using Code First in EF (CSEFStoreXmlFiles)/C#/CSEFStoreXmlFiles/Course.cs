/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFStoreXmlFiles
Copyright (c) Microsoft Corporation.

This sample demonstrates how to import/export the XML into/from database using 
Code First in EF.
This file defines the Course class as the Entity type.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;

namespace CSEFStoreXmlFiles
{
    public class Course
    {
        [Key]
        public String CourseID { get; set; }
        public String Title { get; set; }
        public Int32? Credits { get; set; }
        public String Department { get; set; }
    }
}
