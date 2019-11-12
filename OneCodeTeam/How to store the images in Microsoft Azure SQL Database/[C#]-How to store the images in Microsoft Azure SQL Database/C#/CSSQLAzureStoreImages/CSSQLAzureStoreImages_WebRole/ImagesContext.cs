/****************************** Module Header ******************************\
Module Name:  ImagesContext.cs
Project:      CSSQLAzureStoreImages_WebRole
Copyright (c) Microsoft Corporation.

This sample demonstrates how to store images in Windows Azure SQL Server.
This file includes the Context of Entity Framework.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data.Entity;

namespace CSSQLAzureStoreImages_WebRole
{
    public class ImagesContext : DbContext
    {
        public DbSet<ImageInBlob> BlobImages { get; set; }

        public DbSet<ImageInSQLAzure> SQLAzureImages { get; set; }

        public DbSet<ImagesTable> ImagesTable { get; set; }
    }
}