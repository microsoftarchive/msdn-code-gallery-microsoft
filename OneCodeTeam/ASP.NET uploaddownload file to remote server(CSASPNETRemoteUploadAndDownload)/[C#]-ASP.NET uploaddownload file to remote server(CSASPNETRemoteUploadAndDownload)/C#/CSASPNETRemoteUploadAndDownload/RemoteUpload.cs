/****************************** Module Header ******************************\
* Module Name:  RemoteUpload.cs
* Project:      CSASPNETRemoteUploadAndDownload
* Copyright (c) Microsoft Corporation.
* 
* The CSASPNETRemoteUploadAndDownload sample shows how to upload files to and 
* download files from remote server in an ASP.NET application. 
* 
* This project is created by using WebClient and FtpWebRequest object in C# 
* language. Both WebClient and FtpWebRequest classes provide common methods 
* for sending data to URI of server and receiving data from a resource 
* identified by URI as well. When uploading or downloading files, these 
* classes will do webrequest to the URL which user types in.
* 
* The UploadData() method sends a data buffer(without encoding it) to a 
* resource using HTTP or FTP method specified in the method parameter, and 
* then returns the web response from the server. The DownloadData() method 
* posts an HTTP or FTP download request to the remote server and get 
* outputstream from the server.
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
using System.Net;
using System.IO;


namespace CSRemoteUploadAndDownload
{
    public abstract class RemoteUpload
    {
        public string FileName
        {
            get;
            set;
        }

        public string UrlString
        {
            get;
            set;
        }

        public string NewFileName
        {
            get;
            set;
        }

        public byte[] FileData
        {
            get;
            set;
        }

        public RemoteUpload(byte[] fileData, string fileName, string urlString)
        {
            this.FileData = fileData;
            this.FileName = fileName;
            this.UrlString = urlString.EndsWith("/") ? urlString : urlString + "/";
            string newFileName = DateTime.Now.ToString("yyMMddhhmmss") + 
                DateTime.Now.Millisecond.ToString() + Path.GetExtension(this.FileName);
            this.UrlString = this.UrlString + newFileName;
        }

        /// <summary>
        /// upload file to remote server
        /// </summary>
        /// <returns></returns>
        public virtual bool UploadFile()
        {
            return true;
        }
    }

    /// <summary>
    /// HttpUpload class
    /// </summary>
    public class HttpRemoteUpload : RemoteUpload
    {
        public HttpRemoteUpload(byte[] fileData, string fileNamePath, string urlString)
            : base(fileData, fileNamePath, urlString)
        {
        }

        public override bool UploadFile()
        {
            byte[] postData;
            try
            {
                postData = this.FileData;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.UploadData(this.UrlString, "PUT", postData);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload", ex.InnerException);
            }

        }
    }

    /// <summary>
    /// FtpUpload class
    /// </summary>
    public class FtpRemoteUpload : RemoteUpload
    {
        public FtpRemoteUpload(byte[] fileData, string fileNamePath, string urlString)
            : base(fileData, fileNamePath, urlString)
        {
        }

        public override bool UploadFile()
        {
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(this.UrlString);
            reqFTP.KeepAlive = true;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = this.FileData.Length;

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            MemoryStream ms = new MemoryStream(this.FileData);

            try
            {
                int contenctLength;
                using (Stream strm = reqFTP.GetRequestStream())
                {
                    contenctLength = ms.Read(buff, 0, buffLength);
                    while (contenctLength > 0)
                    {
                        strm.Write(buff, 0, contenctLength);
                        contenctLength = ms.Read(buff, 0, buffLength);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload", ex.InnerException);
            }
        }

    }
}