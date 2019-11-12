/****************************** Module Header ******************************\
 * Module Name:  FTPClient.cs
 * Project:	     CSWindowsStoreAppFTPDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This class is used to download files from a FTP server using WebRequest. 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CSWindowsStoreAppFTPDownloader.FTP
{
    public static class FTPClient
    {
        /// <summary>
        /// Download a single file from FTP server using WebRequest.
        /// </summary>
        public static async Task<DownloadCompletedEventArgs> DownloadFTPFileAsync(FTPFileSystem item,
            StorageFile targetFile, ICredentials credential)
        {
            var result = new DownloadCompletedEventArgs
            {
                RequestFile = item.Url,
                LocalFile = targetFile,
                Error=null 
            };

            // This request is FtpWebRequest in fact.
            WebRequest request = WebRequest.Create(item.Url);
            
            if (credential != null)
            {
                request.Credentials = credential;
            }

            request.Proxy = WebRequest.DefaultWebProxy;

            // Set the method to Download File
            request.Method = "RETR";
            try
            {
                // Open the file for write.
                using (IRandomAccessStream fileStream =
                      await targetFile.OpenAsync(FileAccessMode.ReadWrite))
                {

                    // Get response.
                    using (WebResponse response = await request.GetResponseAsync())
                    {

                        // Get response stream.
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            byte[] downloadBuffer = new byte[2048];
                            int bytesSize = 0;

                            // Download the file until the download is completed.
                            while (true)
                            {

                                // Read a buffer of data from the stream.
                                bytesSize = responseStream.Read(downloadBuffer, 0,
                                    downloadBuffer.Length);
                                if (bytesSize == 0)
                                {
                                    break;
                                }

                                // Write buffer to the file.
                                await fileStream.WriteAsync(downloadBuffer.AsBuffer());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error=ex;
            }

            return result; 
        }
    }
}
