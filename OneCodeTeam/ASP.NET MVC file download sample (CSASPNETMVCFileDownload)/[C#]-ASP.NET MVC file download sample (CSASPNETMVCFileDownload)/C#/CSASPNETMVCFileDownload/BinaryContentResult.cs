/****************************** Module Header ******************************\
Module Name:  BinaryContentResult.cs
Project:      CSASPNETMVCFileDownload
Copyright (c) Microsoft Corporation.

The BinaryContentResult class encapsulates the custom ActionResult class used 
for outputting file content (as binary format)

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSASPNETMVCFileDownload
{
    public class BinaryContentResult : ActionResult
    {
        public BinaryContentResult()
        {
        }

        // Properties for encapsulating http headers.
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        // The code sets the http headers and outputs the file content.
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ClearContent();
            context.HttpContext.Response.ContentType = ContentType;

            context.HttpContext.Response.AddHeader("content-disposition", 
                "attachment; filename=" + FileName);

            context.HttpContext.Response.BinaryWrite(Content);
            context.HttpContext.Response.End();
        }
    }
}
