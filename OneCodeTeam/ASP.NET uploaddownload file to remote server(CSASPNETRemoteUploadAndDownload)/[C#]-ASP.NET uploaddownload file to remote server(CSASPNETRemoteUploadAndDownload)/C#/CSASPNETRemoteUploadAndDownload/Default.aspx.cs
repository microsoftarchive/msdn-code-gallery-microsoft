/****************************** Module Header ******************************\
* Module Name:  Default.aspx.cs
* Project:      CSASPNETRemoteUploadAndDownload
* Copyright (c) Microsoft Corporation.
* 
* Create RemoteUpload instance, input the parameter of Upload file name and 
* server url address. 
* Use UploadFile method to upload file to remote server.
* 
* Create RemoteDownload instance, input the parameter of Download file name 
* and server url address.
* Use DownloadFile method to download file from remote server.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;


namespace CSRemoteUploadAndDownload
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            RemoteUpload uploadClient = null;

            if (this.rbUploadList.SelectedIndex == 0)
            {
                uploadClient = new HttpRemoteUpload(this.FileUpload.FileBytes, 
                    this.FileUpload.PostedFile.FileName, this.tbUploadUrl.Text);
            }
            else
            {
                uploadClient = new FtpRemoteUpload(this.FileUpload.FileBytes, 
                    this.FileUpload.PostedFile.FileName, this.tbUploadUrl.Text);
            }

            if (uploadClient.UploadFile())
            {
                Response.Write("Upload is complete");
            }
            else
            {
                Response.Write("Failed to upload");
            }

        }

        protected void btnDownLoad_Click(object sender, EventArgs e)
        {

            RemoteDownload downloadClient = null;

            if (this.rbDownloadList.SelectedIndex == 0)
            {
                downloadClient = new HttpRemoteDownload(this.tbDownloadUrl.Text, 
                    this.tbDownloadPath.Text);
            }
            else
            {
                downloadClient = new FtpRemoteDownload(this.tbDownloadUrl.Text,
                    this.tbDownloadPath.Text);
            }

            if (downloadClient.DownloadFile())
            {
                Response.Write("Download is complete");
            }
            else
            {
                Response.Write("Failed to download");
            }
        }
    }
}