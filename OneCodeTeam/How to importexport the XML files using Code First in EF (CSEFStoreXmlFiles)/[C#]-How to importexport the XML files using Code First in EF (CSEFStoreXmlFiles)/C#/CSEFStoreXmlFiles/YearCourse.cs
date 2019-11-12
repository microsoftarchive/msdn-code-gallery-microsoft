/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFStoreXmlFiles
Copyright (c) Microsoft Corporation.

This sample demonstrates how to import/export the XML into/from database using 
Code First in EF.
This file defines the YearCourse class as the Entity type.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CSEFStoreXmlFiles
{
    public class YearCourse
    {
        public Int32 YearCourseId { get; set; }
        public Int32 Year { get; set; }

        // The XmlColumn type in the SqlServer will be mapped as the String type 
        // in the EntityFramework.
        [Column(TypeName="xml")]
        public String XmlValues { get; set; }

        // EntityFramework can't map the XDcoument type into the SqlServer type, 
        // so we set NotMapped.
        // We use Xml Values to store and get the Xml document in the database. 
        // And then we use the Courses property to convert the XDocument with 
        // String and access the Xml document. 
        [NotMapped]
        public XDocument Courses
        {
            get { return XDocument.Parse(XmlValues); }
            set { XmlValues = value.ToString(); }
        }
    }
}
