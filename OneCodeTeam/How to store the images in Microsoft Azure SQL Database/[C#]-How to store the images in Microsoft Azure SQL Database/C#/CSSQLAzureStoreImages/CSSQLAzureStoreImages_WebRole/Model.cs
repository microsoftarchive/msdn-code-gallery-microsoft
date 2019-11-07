/****************************** Module Header ******************************\
Module Name:  Model.cs
Project:      CSSQLAzureStoreImages_WebRole
Copyright (c) Microsoft Corporation.

This sample demonstrates how to store images in Windows Azure SQL Server.
This file includes the model of Entity Framework.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CSSQLAzureStoreImages_WebRole
{
    /// <summary>
    /// If we store the image in the blog, we will use this table to store 
    /// the info of image and uri of Blob.
    /// </summary>
    public class ImageInBlob
    {
        [Key]
        public Int32 ImageId { get; set; }
        public String FileName { get; set; }
        public String ImageName { get; set; }
        public String Description { get; set; }
        public String BlobUri { get; set; }
    }

    /// <summary>
    /// If we store the image in the SQL Azure, we will use this table to 
    /// store the info of the image.
    /// </summary>
    public class ImageInSQLAzure
    {
        [Key]
        public Int32 ImageId { get; set; }
        public String FileName { get; set; }
        public String ImageName { get; set; }
        public String Description { get; set; }
    }

    /// <summary>
    /// If we store the image in the SQL Azure we will use this table to 
    /// store the data of the image.
    /// </summary>
    public class ImagesTable
    {
        [Key]
        public Int32 Id { get; set; }

        [Column(TypeName = "image")]
        public byte[] ImageData { get; set; }

        public Int32 ImageId { get; set; }
        [ForeignKey("ImageId")]
        public ImageInSQLAzure ImageInfo { get; set; }
    }
}