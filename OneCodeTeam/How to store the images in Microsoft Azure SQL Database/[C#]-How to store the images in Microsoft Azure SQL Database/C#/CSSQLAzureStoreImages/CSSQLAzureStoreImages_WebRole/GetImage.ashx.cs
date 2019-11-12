/****************************** Module Header ******************************\
Module Name:  GetImage.ashx.cs
Project:      CSSQLAzureStoreImages_WebRole
Copyright (c) Microsoft Corporation.

This sample demonstrates how to store images in Windows Azure SQL Server.
We can use this class to get the images from SQL Azure and return to the client.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSSQLAzureStoreImages_WebRole
{
    /// <summary>
    /// Use it to get the images from SQL Azure.
    /// </summary>
    public class GetImage : IHttpHandler
    {
        ImagesContext imagesDb = new ImagesContext();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";

            Int32 imageId = Int32.Parse(context.Request.QueryString["ImageId"]);
            ImagesTable image = (from i in imagesDb.ImagesTable
                                 where i.ImageId == imageId
                                 select i).FirstOrDefault();
            if (image != null)
            {
                context.Response.BinaryWrite(image.ImageData);
            }
            else
            {
                context.Response.Write("No this Image");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}