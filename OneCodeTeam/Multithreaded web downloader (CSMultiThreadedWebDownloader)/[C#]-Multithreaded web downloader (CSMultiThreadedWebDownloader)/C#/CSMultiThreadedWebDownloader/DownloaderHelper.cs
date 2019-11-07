/****************************** Module Header ******************************\
 * Module Name:  DownloaderHelper.cs
 * Project:      CSMultiThreadedWebDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * This class supplies the methods to 
 * 1. Initialize a HttpWebRequest object. 
 * 2. Check the url and initialize some properties of a downloader.
 * 3. Check whether the destination file exists. If not, create a file with 
 *    the same size as the file to be downloaded.
 *  
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
using System.Text.RegularExpressions;
using System.IO;

namespace CSMultiThreadedWebDownloader
{
    public static class DownloaderHelper
    {

        public static HttpWebRequest InitializeHttpWebRequest(IDownloader downloader)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(downloader.Url);

            if (downloader.Credentials != null)
            {
                webRequest.Credentials = downloader.Credentials;
            }
            else
            {
                webRequest.Credentials = CredentialCache.DefaultCredentials;
            }

            if (downloader.Proxy != null)
            {
                webRequest.Proxy = downloader.Proxy;
            }
            else
            {
                webRequest.Proxy = WebRequest.DefaultWebProxy;
            }

            return webRequest;
        }

        /// <summary>
        /// Check the URL to download, including whether it supports Range, 
        /// </summary>
        /// <param name="downloader"></param>
        /// <returns></returns>
        public static string CheckUrl(IDownloader downloader)
        {
            string fileName = string.Empty;

            // Check the file information on the remote server.
            var webRequest = InitializeHttpWebRequest(downloader);

            using (var response = webRequest.GetResponse())
            {
                foreach (var header in response.Headers.AllKeys)
                {
                    if (header.Equals("Accept-Ranges", StringComparison.OrdinalIgnoreCase))
                    {
                        downloader.IsRangeSupported = true;
                    }

                    if (header.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
                    {
                        string contentDisposition = response.Headers[header];

                        string pattern = ".[^;]*;\\s+filename=\"(?<file>.*)\"";
                        Regex r = new Regex(pattern);
                        Match m = r.Match(contentDisposition);
                        if (m.Success)
                        {
                            fileName = m.Groups["file"].Value;
                        }
                    }
                }

                downloader.TotalSize = response.ContentLength;

                if (downloader.TotalSize <= 0)
                {
                    throw new ApplicationException(
                        "The file to download does not exist!");
                }

                if (!downloader.IsRangeSupported)
                {
                    downloader.StartPoint = 0;
                    downloader.EndPoint = int.MaxValue;
                }
            }

            if (downloader.IsRangeSupported &&
                (downloader.StartPoint != 0 || downloader.EndPoint != long.MaxValue))
            {
                webRequest = InitializeHttpWebRequest(downloader);

                if (downloader.EndPoint != int.MaxValue)
                {
                    webRequest.AddRange(downloader.StartPoint, downloader.EndPoint);
                }
                else
                {
                    webRequest.AddRange(downloader.StartPoint);
                }
                using (var response = webRequest.GetResponse())
                {
                    downloader.TotalSize = response.ContentLength;
                }
            }

            return fileName;
        }


        /// <summary>
        /// Check whether the destination file exists. If not, create a file with the same
        /// size as the file to be downloaded.
        /// </summary>
        public static void CheckFileOrCreateFile(IDownloader downloader, object fileLocker)
        {
            // Lock other threads or processes to prevent from creating the file.
            lock (fileLocker)
            {
                FileInfo fileToDownload = new FileInfo(downloader.DownloadPath);
                if (fileToDownload.Exists)
                {

                    // The destination file should have the same size as the file to be downloaded.
                    if (fileToDownload.Length != downloader.TotalSize)
                    {
                        throw new ApplicationException(
                            "The download path already has a file which does not match"
                            + " the file to download. ");
                    }
                }

                // Create a file.
                else
                {
                    if (downloader.TotalSize == 0)
                    {
                        throw new ApplicationException("The file to download does not exist!");
                    }

                    using (FileStream fileStream = File.Create(downloader.DownloadPath))
                    {
                        long createdSize = 0;
                        byte[] buffer = new byte[4096];
                        while (createdSize < downloader.TotalSize)
                        {
                            int bufferSize = (downloader.TotalSize - createdSize) < 4096
                                ? (int)(downloader.TotalSize - createdSize) : 4096;
                            fileStream.Write(buffer, 0, bufferSize);
                            createdSize += bufferSize;
                        }
                    }
                }
            }
        }
    }
}
