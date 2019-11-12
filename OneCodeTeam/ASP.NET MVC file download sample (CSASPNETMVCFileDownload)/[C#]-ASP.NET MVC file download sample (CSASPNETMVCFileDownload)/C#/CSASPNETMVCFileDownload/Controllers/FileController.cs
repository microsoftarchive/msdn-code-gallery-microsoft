/****************************** Module Header ******************************\
Module Name:  FileController.cs
Project:      CSASPNETMVCFileDownload
Copyright (c) Microsoft Corporation.

This module contains the FileController class. 

FileController is the controller dedicated for file downloading functionality.
For request to list file, FileController will call List Action to return the 
file list and display it via File/List view

File request to download a certain file, FileController will call the 
Download action to return the stream of the requested file.

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
using System.Web.Mvc.Ajax;
using System.IO;


namespace CSASPNETMVCFileDownload.Controllers
{
    public class FileController : Controller
    {
        // Action for list all the files in "~/App_Data/download" directory
        public ActionResult List()
        {
            // Retrieve the file list.
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/App_Data/download/"));

            // Filter it via LINQ to Object.
            var files = from f in dir.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                        where f.Extension != "exe"
                        select f;

            // Call the corresponding View.
            return View(files.ToList());
        }

        // Action for returning the binary stream of a specified file.
        public ActionResult Download(string fn)
        {
            // Check whether the requested file is valid.
            string pfn = Server.MapPath("~/App_Data/download/" + fn);
            if (!System.IO.File.Exists(pfn))
            {
                throw new ArgumentException("Invalid file name or file not exists!");
            }

            // Use BinaryContentResult to encapsulate the file content and return it.
            return new BinaryContentResult()
            {
                FileName = fn,
                ContentType = "application/octet-stream",
                Content = System.IO.File.ReadAllBytes(pfn)
            };
        }
    }
}
