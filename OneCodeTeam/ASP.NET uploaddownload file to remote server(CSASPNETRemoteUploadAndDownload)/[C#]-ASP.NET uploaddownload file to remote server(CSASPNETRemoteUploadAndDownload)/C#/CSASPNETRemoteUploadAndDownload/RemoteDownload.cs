/****************************** Module Header ******************************\
* Module Name:  RemoteDownload.cs
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
    public abstract class RemoteDownload
    {
        public string UrlString
        {
            get;
            set;
        }

        public string DestDir
        {
            get;
            set;
        }

        public RemoteDownload(string urlString, string destDir)
        {
            this.UrlString = urlString;
            this.DestDir = destDir;
        }

        ///<summary>
        /// Download file from remote server
        ///</summary>
        public virtual bool DownloadFile()
        {
            return true;
        }
    }

    /// <summary>
    /// HttpRemoteDownload class
    /// </summary>
    public class HttpRemoteDownload : RemoteDownload
    {
        public HttpRemoteDownload(string urlString, string descFilePath)
            : base(urlString, descFilePath)
        {
        }

        public override bool DownloadFile()
        {
            string fileName = Path.GetFileName(this.UrlString);
            string descFilePath = Path.Combine(this.DestDir, fileName);
            try
            {
                WebRequest myre = WebRequest.Create(this.UrlString);
            }
            catch (Exception ex)
            {
                throw new Exception("File doesn't exist on server", ex.InnerException);
            }
            try
            {
                byte[] fileData;
                using (WebClient client = new WebClient())
                {
                    fileData = client.DownloadData(this.UrlString);
                }
                using (FileStream fs = new FileStream(descFilePath, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download", ex.InnerException);
            }
        }
    }

    /// <summary>
    /// FtpDownload class
    /// </summary>
    public class FtpRemoteDownload : RemoteDownload
    {
        public FtpRemoteDownload(string urlString, string descFilePath)
            : base(urlString, descFilePath)
        {
        }

        public override bool DownloadFile()
        {
            FtpWebRequest reqFTP;

            string fileName = Path.GetFileName(this.UrlString);
            string descFilePath = Path.Combine(this.DestDir, fileName);

            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(this.UrlString);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;

                using (FileStream outputStream = new FileStream(descFilePath, FileMode.OpenOrCreate))
                {
                    using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                    {
                        using (Stream ftpStream = response.GetResponseStream())
                        {
                            int bufferSize = 2048;
                            int readCount;
                            byte[] buffer = new byte[bufferSize];
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                            while (readCount > 0)
                            {
                                outputStream.Write(buffer, 0, readCount);
                                readCount = ftpStream.Read(buffer, 0, bufferSize);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download", ex.InnerException);
            }
        }
    }
}