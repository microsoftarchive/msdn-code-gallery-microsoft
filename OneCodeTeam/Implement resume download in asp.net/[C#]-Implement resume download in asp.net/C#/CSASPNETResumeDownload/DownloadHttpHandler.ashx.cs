/****************************** Module Header ******************************\
* Module Name: DownloadHttpHandler.ashx.cs
* Project:     CSASPNETResumeDownload
* Copyright (c) Microsoft Corporation
*
* This project demonstrates how to implement resume download feature in Asp.net.
* As we know, due to network interruptions, downloading a file is a problem when 
* the size of the file is large, we need to support resume download if the connection
* broken.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Configuration;
using System.Web;

namespace CSASPNETResumeDownload
{
    public class DownloadHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string filePath = ConfigurationManager.AppSettings["FilePath"];
            Downloader.DownloadFile(context, filePath);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
