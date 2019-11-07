/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFStoreXmlFiles
Copyright (c) Microsoft Corporation.

This sample demonstrates how to import/export the XML into/from database using 
Code First in EF.
This file defines the MySchoolContext class.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data.Entity;

namespace CSEFStoreXmlFiles
{
    public class MySchoolContext:DbContext
    {
        public MySchoolContext()
        { }

        /// <summary>
        /// We can specify the database name the context creates or maps.
        /// </summary>
        /// <param name="databaseName">the database name the context creates or maps</param>
        public MySchoolContext(String databaseName)
            : base(databaseName)
        { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<YearCourse> YearCourses { get; set; }
    }
}
